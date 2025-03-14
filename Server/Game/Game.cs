using Microsoft.AspNetCore.SignalR;
using Server.Game.Entities;
using Server.Game.Interface;
using Server.Game.Npc;
using Server.Map;
using Server.Services;
using Server.Game.Npc;
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
    public int MonsterAmount { get; init; }
    public double MonsterSpeed { get; init; }
    public int BombDelay { get; init; }
}
public class Game
{
    public int Id { get; set; }
    public bool Live { get; set; }
    private List<IEntity> _entities = [];
    private HubGameService? _hubGameService;
    private readonly object _lockObject = new ();
    private int MonsterAmount { get; set; } = 5;
    public double MonsterSpeed { get; private set; } = 1.3;
    public int BombDelay { get; private set; } = 2000;
    private readonly MonsterFactory _monsterFactory;
    private readonly PlayerHandler _playerHandler = new ();
    private readonly string _groupName;
    public List<ComputerPlayer> Npcs { get; set; } = [];

    public Game(HubGameService hubGameService, string groupName)
    {
        _groupName = groupName;
        _hubGameService = hubGameService;
        _monsterFactory = new MonsterFactory(5, this);
        Players =
        [
            new Player(this){ Id = string.Empty, Live = false},
            new Player(this){ Id = string.Empty, Live = false},
            new Player(this){ Id = string.Empty, Live = false},
            new Player(this){ Id = string.Empty, Live = false}
        ];
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
    public IReadOnlyList<IEntity> GetEntities()
    {
        lock (_lockObject)
        {
            return _entities.ToArray();
        }
    }
    public void AddEntities(IEntity entity)
    {
        lock (_lockObject)
        {
            _entities.Add(entity);
        }
    }

    private void AddEntities(IEnumerable<IEntity> entities)
    {
        lock (_lockObject)
        {
            _entities.AddRange(entities);
        }
    }
    
    private Player[] Players { get; }

    private Player? MasterPlayer => Players.Length != 0 ? Players[0] : null;
    public bool IsMasterPlayer(string id) => MasterPlayer is not null && MasterPlayer.Id == id;
    private Wall[] GenerateMap()
    {
        var map = MapHandler.GenerateMap(this);
        if(map is not null)
            AddEntities(map);
        
        return GetMap();
    }

    public void RemoveEntities(Type? type = null)
    {
        lock (_lockObject)
        {
            if (type is null)
                _entities = [];
            else
                _entities.RemoveAll(e => e.GetType() == type);
        }
    }
    
    public void RemoveEntities<T>()
    {
        lock (_lockObject)
        { 
            _entities.RemoveAll(e => e is T);
        }
    }

    public List<(double X, double Y)> FindAllEmptyCoordinates() => MapHandler.GetEmptySpaces();

    public Wall[] GetMap() => GetEntities().Where(e => e is Wall).Cast<Wall>().ToArray();

    private void GenerateMonsters() => _monsterFactory.GenerateMonsters();
    
    public IEnumerable<IMonster> GetMonsters() => GetEntities().Where( e => e is IMonster).Cast<IMonster>();
    public IEnumerable<IMoveable> GetMoveable() => GetEntities().Where( e => e is IMoveable).Cast<IMoveable>();

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
        AddEntities(newPlayer);
        
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
        if (player != null)
        {
            player.Live = false;
            lock (_lockObject)
            {
                _entities.Remove(player);
            }
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

    public Bomb PlantBomb(double x, double y, Player owner)
    {
        var bomb = new Bomb(x, y, owner, this);
        AddEntities(bomb);
        return bomb;
    }

    public async Task RestartGame()
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
            await SendToAll("Connected", players);
            await SendToAll("GetMap", newMap);
        }
    }

    public async Task SendToPlayer(string methodName, string playerId, object? args)
    {
        var players = Players.Where(p => p.Id == playerId).Select(p => p.Id).ToArray();
        Console.WriteLine($"SendToPlayer {methodName} {playerId} {args} {string.Join("", players)}");
        await _hubGameService?.HubContext.Clients.Clients(players).SendAsync(methodName, args)!;
    }
    
    public async Task SendToAll(string methodName, object? args)
    {
       await _hubGameService?.HubContext.Clients.Group(_groupName).SendAsync(methodName, args)!; 
    }
    
    public async Task SendToAll(string methodName, object? args, object? args2)
    {
        await _hubGameService?.HubContext.Clients.Group(_groupName).SendAsync(methodName, args, args2)!; 
    }
    
    public async Task SendToAll(string methodName, object? args, object? args2, object? args3)
    {
        await _hubGameService?.HubContext.Clients.Group(_groupName).SendAsync(methodName, args, args2, args3)!; 
    }

    internal void CreateBonus(IEntity e)
    {
        var rnd = new Random();
        var result = rnd.Next(1, 4);
        if (result != 1) return;
        
        var bonus = new Bonus(this) { PosX = e.PosX, PosY = e.PosY, Destructible = true };
        AddEntities(bonus);
        _ = SendToAll("SetBonus", bonus);
    }

    public void RemoveDestroyedEntities()
    {
        lock (_lockObject)
        {
            _entities.RemoveAll(e => e.Destroyed);
        }
    }
    
    public void RemoveEntities()
    {
        lock (_lockObject)
        {
            _entities.Clear();
        }
    }

    public void SetHubGameService(HubGameService hubGameService)
    {
        _hubGameService = hubGameService;
    }

    public HubGameService? GetHubGameService()
    {
        return _hubGameService;
    }

    public Player[] GetPlayers() => Players;

    public void Destroy()
    {
        RemoveEntities();
    }

    public Bullet? CreateBullet(Player owner, double sin, double cos)
    {
        if (!(GetBullets()?.Where(b => b.Owner == owner).Count() < owner.MaxBullets)) return null;
        
        var bullet = new Bullet(this, owner, sin, cos);
        AddEntities(bullet);
        return bullet;

    }

    public IEnumerable<IBullet> GetBullets() => GetEntities().Where( e => e is IBullet).Cast<IBullet>();

    public void AddNpcPlayer()
    {
        var player = AddPlayer("NPC1");
        if (player is null) return;
        
        var newNpc = new ComputerPlayer(player);
        Npcs.Add(newNpc);
    }

    public void UpdateNpc()
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
}