using Game.Game.Entities;
using Game.Game.Interface;
using Game.Game.Models;
using Game.Game.Npc;
using Game.Map;
using GameEngine;
using GameEngine.Interface;
using IMoveable = Game.Game.Interface.IMoveable;

namespace Game.Game;

public enum MoveDirection
{
    Right,
    Left,
    Up,
    Down,
    None
}

public enum GameState
{
    InGame,
    InLobby
}
public class GameConfig
{
    public int MonsterAmount { get; init; }
    public double MonsterSpeed { get; init; }
    public int BombDelay { get; init; }
}
public class Game : World, ICommunicateHandler
{
    public GameState GameState { get; set; }
    private Wall[]? Map { get; set; }
    private readonly ICommunicateHandler? _hubGameService;
    private readonly object _lockObject = new ();
    private int MonsterAmount { get; set; } = 5;
    private double MonsterSpeed { get; set; } = 1.3;
    public int BombDelay { get; private set; } = 2000;
    private readonly MonsterFactory _monsterFactory;
    private TimeSpan _sendPlayerLocationsAccumulator = TimeSpan.Zero;
    private static readonly TimeSpan SendPlayerLocationsInterval = TimeSpan.FromMilliseconds(20); // 10 Hz
    public List<ComputerPlayer> Npcs { get; set; } = [];

    public Game(ICommunicateHandler hubGameService, string groupName)
    {
        _hubGameService = hubGameService;
        GroupName = groupName;
        _monsterFactory = new MonsterFactory(5, this);
        Players =
        [
            new Player(this){ Id = string.Empty, Live = false},
            new Player(this){ Id = string.Empty, Live = false},
            new Player(this){ Id = string.Empty, Live = false},
            new Player(this){ Id = string.Empty, Live = false}
        ];
        
        OnUpdate += UpdateNpc;
        OnUpdate += MoveLivePlayers;
        OnUpdate += MoveLiveMonster;
        OnUpdate += MoveLiveBullets;
        OnUpdate += SendPlayerLocations;
        OnUpdate += SendMonstersLocations;
        OnUpdate += RemoveDestroyedEntities;
    }

    public GameConfig GetConfig()
    {
        return new GameConfig { MonsterAmount = MonsterAmount, MonsterSpeed = MonsterSpeed, BombDelay = BombDelay };
    }
    
    public void SetGameConfig(GameConfig config)
    {
        MonsterAmount = _monsterFactory.MonsterAmount = config.MonsterAmount;
        MonsterSpeed = config.MonsterSpeed;
        BombDelay = config.BombDelay;
        foreach (var player in Players)
        {
            player.BombDelay = BombDelay;
        }
    }
    
    private Player[] Players { get; }

    private Player? MasterPlayer => Players.Length != 0 ? Players[0] : null;
    public bool IsMasterPlayer(string id) => MasterPlayer is not null && MasterPlayer.Id == id;
    private Wall[] GenerateMap()
    {
        Map = MapHandler.GenerateMap(this);
        if(Map is not null)
            AddEntities(Map);
        
        return GetMap();
    }

    private void RemoveEntities(Type? type)
    {
        lock (_lockObject)
        {
            if (type is null)
                Entities = [];
            else
                Entities.RemoveAll(e => e.GetType() == type);
        }
    }

    public List<(double X, double Y)> FindAllEmptyCoordinates() => MapHandler.GetEmptySpaces();

    public Wall[] GetMap() => GetEntities().Where(e => e is Wall { Empty: false }).Cast<Wall>().ToArray();
    public Wall[] GetAllMap() =>  GetEntities().OfType<Wall>().ToArray();
    private void GenerateMonsters() => _monsterFactory.GenerateMonsters();
    
    public IEnumerable<IMonster> GetMonsters() => GetEntities().Where( e => e is IMonster).Cast<IMonster>();
    private IEnumerable<IMoveable> GetMoveable() => GetEntities().Where( e => e is IMoveable).Cast<IMoveable>();

    public Player? AddPlayer(string id)
    {
        if (!GetFreePlayerSlotNumber(out var idPlayer)) return null;

        var newPlayer = idPlayer switch
        {
            0 => new Player(this) { Id = id, Name = "mikze", PosX = 101, PosY = 100 },
            1 => new Player(this) { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 },
            2 => new Player(this) { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50 },
            3 => new Player(this) { Id = id, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50 },
            _ => null
        };

        if (newPlayer == null) return null;
        Players[idPlayer] = newPlayer;
        AddEntity(newPlayer);
        
        return newPlayer;
    }

    private bool GetFreePlayerSlotNumber(out int number)
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


    public void RemovePlayer(string id)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player == null) return;
        player.Live = false;
        lock (_lockObject)
        {
            Entities.Remove(player);
        }
    }

    public void ChangeName(string id, string newName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
            player.Name = newName;
    }

    public void ChangeSkin(string id, string newSkinName)
    {
        var targetPlayer = Players.FirstOrDefault(p => p.Id == id);
        if (targetPlayer != null)
            targetPlayer.Skin = newSkinName;
    }

    public async Task RestartGame()
    {
        Pause = true;
        Destroy();
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
            await SendToAll("Connected",GroupName, players);
            await SendToAll("GetMap",GroupName, newMap);
        }
        
        if(GameState == GameState.InGame)
            Pause = false;
    }

    public async Task SendToPlayer(string methodName, string playerId, object? args)
    {
        var players = Players.Where(p => p.Id == playerId).Select(p => p.Id).ToArray();
        Console.WriteLine($"SendToPlayer {methodName} {playerId} {args} {string.Join("", players)}");
        await _hubGameService?.SendToPlayer(methodName, playerId, args)!;
    }
    
    public async Task SendToAll(string methodName, string groupName, object? args)
        =>await _hubGameService?.SendToAll(methodName,groupName, args)!;
    
    
    public async Task SendToAll(string methodName, string groupName, object? args, object? args2)
        => await _hubGameService?.SendToAll(methodName,groupName, args)!;
    
    
    public async Task SendToAll(string methodName, string groupName, object? args, object? args2, object? args3)
        => await _hubGameService?.SendToAll(methodName,groupName, args, args2, args3)!;

    public void CreateBonus(IEntity e)
    {
        var rnd = new Random();
        var result = rnd.Next(1, 4);
        if (result != 1) return;
        
        var bonus = new Bonus(this) { PosX = e.PosX, PosY = e.PosY, Destructible = true };
        AddEntity(bonus);
        _ = SendToAll("SetBonus",GroupName, bonus);
    }

    private void RemoveDestroyedEntities()
    {
        lock (_lockObject)
        {
            Entities.RemoveAll(e => e.Destroyed);
        }
    }
    

    public ICommunicateHandler? GetHubGameService()
    {
        return _hubGameService;
    }

    public Player[] GetPlayers() => Players;

    public Bullet? CreateBullet(Player owner, double sin, double cos)
    {
        if (!(GetBullets().Count(b => b.Owner == owner) < owner.MaxBullets)) return null;
        
        var bullet = new Bullet(this, owner, sin, cos);
        AddEntity(bullet);
        return bullet;

    }

    private IEnumerable<IBullet> GetBullets() => GetEntities().Where( e => e is IBullet).Cast<IBullet>();

    public void AddNpcPlayer()
    {
        var player = AddPlayer("NPC1");
        if (player is null) return;
        
        var newNpc = new ComputerPlayer(player);
        Npcs.Add(newNpc);
        
        // Choose some destination tile (e.g., random reachable)
        var empties = FindAllEmptyCoordinates(); // returns (X, Y)
        if (empties.Count <= 0) return;
        
        Console.WriteLine("Empties are present");
        var (x, y) = empties[Random.Shared.Next(empties.Count)];
        newNpc.CreateDest((int)y, (int)x);
    }

    private void UpdateNpc(TimeSpan delta)
    {
        foreach (var npc in Npcs)
            npc.Update();
    }

    public List<(double, double)> GetSafePositionsNear((double PosX, double PosY) valueTuple)
    {
        // Extract X and Y from the input tuple
        var (posX, posY) = valueTuple;

        // Set a danger radius (e.g., how far bombs or hazards affect an area)
        const double dangerRadius = 2.0;

        // Get all empty coordinates from the map
        var emptyPositions = FindAllEmptyCoordinates();

        // Determine which empty positions are outside of the danger radius
        var safePositions = emptyPositions
            .Where(coord => 
            {
                // Calculate distance from the given position
                var distance = Math.Sqrt(Math.Pow(coord.X - posX, 2) + Math.Pow(coord.Y - posY, 2));
                return distance > dangerRadius;
            })
            .ToList();

        // Return the list of safe positions
        return safePositions;
    }

    public bool IsObstacle(double p0, double playerPosY)
    {
        // Retrieve all Walls (obstacles) in the game.
        var walls = GetMap();

        // Check if the given position matches any Wall's position.
        foreach (var wall in walls)
        {
            // If the coordinates match, this position is an obstacle.
            if (Math.Abs(wall.PosX - p0) < 0.1 && Math.Abs(wall.PosY - playerPosY) < 0.1)
            {
                return true;
            }
        }

        // No matching Wall means the position is not an obstacle.
        return false;
    }
    
    
    private void SendMonstersLocations(TimeSpan delta)
    {
        _sendPlayerLocationsAccumulator += delta;
        if (_sendPlayerLocationsAccumulator < SendPlayerLocationsInterval)
            return;

        // keep leftover time to reduce drift (important if delta varies)
        _sendPlayerLocationsAccumulator -= SendPlayerLocationsInterval;

        var monsters = GetEntities<BasicMonster>().Where(p => p is { Destroyed: false });
        _ = SendToAll("MoveMonsters",GroupName, monsters);
    }
    private void MoveLiveMonster(TimeSpan delta)
    {
        foreach (var monster in GetMonsters().Where(p => p is { Destroyed: false }))
            monster.Move(MoveDirection.None);
    }
    
    private void MoveLiveBullets(TimeSpan delta)
    {
        foreach (var monster in GetBullets().Where(p => p is { Destroyed: false }))
            monster.Move(MoveDirection.None);
    }
    
    private void MoveLivePlayers(TimeSpan delta)
    {
        foreach (var player in GetPlayers().Where(p =>
                     p is { Live: true, Dead: false } && p.MoveDirection != MoveDirection.None))
        {
            player.Move(player.MoveDirection);
        }
    }
    
    private void SendPlayerLocations(TimeSpan delta)
    {
        _sendPlayerLocationsAccumulator += delta;
        if (_sendPlayerLocationsAccumulator < SendPlayerLocationsInterval)
            return;

        // keep leftover time to reduce drift (important if delta varies)
        _sendPlayerLocationsAccumulator -= SendPlayerLocationsInterval;

        var playerModels = GetPlayers().Select(p => new PlayerModel(p)).ToArray();
        _ = SendToAll("MovePlayer", GroupName, playerModels);
    }
    
    private void RemoveDestroyedEntities(TimeSpan delta)
    {
        foreach (var entity in GetEntities().Where(e => e.Destroyed))
        {
            _ = SendToAll("RemoveEntity",GroupName, entity.Id);
        }

        RemoveDestroyedEntities();
    }
}