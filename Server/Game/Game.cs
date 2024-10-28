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
public static class Game
{
    public static bool Live { get; set; }
    public static List<IEntity> Entities = new();
    public static HubGameService? HubGameService;
    public static readonly object LockObject = new ();
    public static IReadOnlyList<IEntity> GetEntities()
    {
        lock (LockObject)
        {
            return Entities.ToArray();
        }
    }
    private static void AddEntities(IEntity entity)
    {
        lock (LockObject)
        {
            Entities.Add(entity);
        }
    }
    
    private static void AddEntities(IEnumerable<IEntity> entities)
    {
        lock (LockObject)
        {
            Entities.AddRange(entities);
        }
    }
    
    public static Player[] Players { get; } =
    [
        new (){ Id = string.Empty, Live = false},
        new (){ Id = string.Empty, Live = false},
        new (){ Id = string.Empty, Live = false},
        new (){ Id = string.Empty, Live = false}
    ];

    private static Player? MasterPlayer => Players.Any() ? Players[0] : null;
    public static bool IsMasterPlayer(string id) => MasterPlayer is not null && MasterPlayer.Id == id;
    private static Wall[] GenerateMap()
    {
        var map = MapHandler.GenerateMap();
        if(map is not null)
            AddEntities(map);
        
        return GetMap();
    }

    private static void RemoveEntities(Type? type = null)
    {
        lock (LockObject)
        {
            if (type is null)
                Entities = [];
            else
                Entities.RemoveAll(e => e.GetType() == type);
        }
    }

    public static Wall[] GetMap() => GetEntities().Where(e => e is Wall).Cast<Wall>().ToArray();

    private static void GenerateMosters()
    {
        RemoveEntities(typeof(Monster));
        AddEntities(new Monster(){ PosX = 159, PosY = 200 });
    }

    private static IEnumerable<Monster> GetMonsters() => GetEntities().Where( e => e is Monster).Cast<Monster>();

    public static void AddPlayer(string id)
    {
        if (GetFreePlayerSlotNumber(out int idPlayer))
        {
            Player? newPlayer = null;

            switch (idPlayer)
            {
                case 0:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 101, PosY = 100 };
                    break;
                case 1:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 };
                    break;
                case 2:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50 };
                    break;
                case 3:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50 };
                    break;
            }

            if (newPlayer != null)
            {
                Players[idPlayer] = newPlayer;
                AddEntities(newPlayer);
            }
        }
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
                Entities.Remove(player);
            }
        }
    }

    public static void MovePlayer(string id, MoveDirection moveDirection)
    {
        Thread.Sleep(5);
        var player = Players.FirstOrDefault(p => p.Id == id);
        player?.MovePlayer(moveDirection);
    }

    public static void ChangeName(string id, string newName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
            player.Name = newName;
    }

    public static void ChangeSkin(string id, string newSkinName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
        {
        }
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
        GenerateMosters();

        var newMap = GenerateMap();
        var players = Players.Where(p => p.Live).ToArray();
        var monsters = GetMonsters();

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

        if (HubGameService != null)
        {
            await HubGameService.HubContext.Clients.All.SendAsync("Connected", players);
            await HubGameService.HubContext.Clients.All.SendAsync("GetMap", newMap);
        }
    }

    internal static void CreateBonus(IEntity e)
    {
        var rnd = new Random();
        var result = rnd.Next(1, 4);
        if (result != 1) return;
        
        var bonus = new Bonus() { PosX = e.PosX, PosY = e.PosY, Destructible = true };
        AddEntities(bonus);
        HubGameService?.HubContext.Clients.All.SendAsync("SetBonus", bonus);
    }
}