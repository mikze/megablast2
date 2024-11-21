using Microsoft.AspNetCore.SignalR;
using Server.Game.Entities;
using Server.Game.Interface;
using Server.Map;
using Server.Services;

namespace Server.Game;

public enum MoveDirection
{
    Right,
    Left,
    Up,
    Down,
    None
}
public class GameConfig
{
    public int MonsterAmount { get; set; }
    public double MonsterSpeed { get; set; }
    public int BombDelay { get; set; }
}
public static class Game
{
    
    public static bool Live { get; set; }
    private static List<IEntity> _entities = [];
    private static HubGameService? _hubGameService;
    public static readonly object LockObject = new ();
    private static int MonsterAmount { get; set; } = 5;
    public static double MonsterSpeed { get; set; } = 1.3;
    public static int BombDelay { get; set; } = 2000;
    private static readonly MonsterFactory MonsterFactory = new (MonsterAmount);
    private static readonly PlayerHandler PlayerHandler = new ();

    public static GameConfig GetConfig()
    {
        return new GameConfig { MonsterAmount = MonsterAmount, MonsterSpeed = MonsterSpeed, BombDelay = BombDelay };
    }
    
    public static void SetGameConfig(GameConfig config)
    {
        MonsterAmount = MonsterFactory.MonsterAmount = config.MonsterAmount;
        MonsterSpeed = config.MonsterSpeed;
        BombDelay = config.BombDelay;
        foreach (var player in Players)
        {
            player.BombDelay = BombDelay;
        }
    }
    public static IReadOnlyList<IEntity> GetEntities()
    {
        lock (LockObject)
        {
            return _entities.ToArray();
        }
    }
    public static void AddEntities(IEntity entity)
    {
        lock (LockObject)
        {
            _entities.Add(entity);
        }
    }

    private static void AddEntities(IEnumerable<IEntity> entities)
    {
        lock (LockObject)
        {
            _entities.AddRange(entities);
        }
    }
    
    private static Player[] Players { get; } =
    [
        new (){ Id = string.Empty, Live = false},
        new (){ Id = string.Empty, Live = false},
        new (){ Id = string.Empty, Live = false},
        new (){ Id = string.Empty, Live = false}
    ];

    private static Player? MasterPlayer => Players.Length != 0 ? Players[0] : null;
    public static bool IsMasterPlayer(string id) => MasterPlayer is not null && MasterPlayer.Id == id;
    private static Wall[] GenerateMap()
    {
        var map = MapHandler.GenerateMap();
        if(map is not null)
            AddEntities(map);
        
        return GetMap();
    }

    public static void RemoveEntities(Type? type = null)
    {
        lock (LockObject)
        {
            if (type is null)
                _entities = [];
            else
                _entities.RemoveAll(e => e.GetType() == type);
        }
    }

    public static List<(double X, double Y)> FindAllEmptyCoordinates() => MapHandler.GetEmptySpaces();

    public static Wall[] GetMap() => GetEntities().Where(e => e is Wall).Cast<Wall>().ToArray();

    private static void GenerateMonsters() => MonsterFactory.GenerateMonsters();
    
    public static IEnumerable<IMonster> GetMonsters() => GetEntities().Where( e => e is IMonster).Cast<IMonster>();

    public static void AddPlayer(string id)
    {
        if (!GetFreePlayerSlotNumber(out var idPlayer)) return;

        var newPlayer = idPlayer switch
        {
            0 => new Player { Id = id, Name = "mikze", PosX = 101, PosY = 100 },
            1 => new Player { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 },
            2 => new Player { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50 },
            3 => new Player { Id = id, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50 },
            _ => null
        };

        if (newPlayer == null) return;
        Players[idPlayer] = newPlayer;
        AddEntities(newPlayer);
    }

    private static bool GetFreePlayerSlotNumber(out int number)
    {
        for (var i = 0; i < 4; i++)
        {
            if (Players[i].Live) continue;
            number = i;
            return true;
        }
        number = -1;
        return false;
    }


    public static void RemovePlayer(string id)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
        {
            player.Live = false;
            lock (LockObject)
            {
                _entities.Remove(player);
            }
        }
    }

    public static void ChangeName(string id, string newName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
            player.Name = newName;
    }

    public static void ChangeSkin(string id, string newSkinName)
    {
        var targetPlayer = Players.FirstOrDefault(p => p.Id == id);
        if (targetPlayer != null)
            targetPlayer.Skin = newSkinName;
    }

    public static Bomb PlantBomb(double x, double y, Player owner)
    {
        var bomb = new Bomb(x, y, owner);
        AddEntities(bomb);
        return bomb;
    }

    public static async Task RestartGame()
    {
        RemoveEntities(typeof(Wall));
        var newMap = GenerateMap();
        GenerateMonsters();
        var players = Players.Where(p => p.Live).ToArray();

        for (var i = 0; i < players.Length; i++)
        {
            switch (i)
            {
                case 0:
                    players[0].PosX = 101;
                    players[0].PosY = 100;
                    players[0].Dead = false;
                    break;
                case 1:
                    players[1].PosX = 99 + 14 * 50;
                    players[1].PosY = 100;
                    players[1].Dead = false;
                    break;
                case 2:
                    players[2].PosX = 99 + 14 * 50;
                    players[2].PosY = 99 + 13 * 50;
                    players[2].Dead = false;
                    break;
                case 3:
                    players[3].PosX = 101;
                    players[3].PosY = 99 + 13 * 50;
                    players[3].Dead = false;
                    break;
            }
        }

        if (_hubGameService != null)
        {
            await _hubGameService.HubContext.Clients.All.SendAsync("Connected", players);
            await _hubGameService.HubContext.Clients.All.SendAsync("GetMap", newMap);
        }
    }

    internal static void CreateBonus(IEntity e)
    {
        var rnd = new Random();
        var result = rnd.Next(1, 4);
        if (result != 1) return;
        
        var bonus = new Bonus() { PosX = e.PosX, PosY = e.PosY, Destructible = true };
        AddEntities(bonus);
        _hubGameService?.HubContext.Clients.All.SendAsync("SetBonus", bonus);
    }

    public static void RemoveDestroyedEntities()
    {
        lock (LockObject)
        {
            _entities.RemoveAll(e => e.Destroyed);
        }
    }

    public static void SetHubGameService(HubGameService hubGameService)
    {
        _hubGameService = hubGameService;
    }

    public static HubGameService? GetHubGameService()
    {
        return _hubGameService;
    }

    public static Player[] GetPlayers() => Players;
    
}