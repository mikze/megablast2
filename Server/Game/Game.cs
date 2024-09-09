using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;

public enum MoveDirection
{
    Right,
    Left,
    Up,
    Down,
    none
}
public static class Game
{
    public static int[][]? Map;
    public static List<IEntity> Entities = new List<IEntity>();
    static object? LockEntities;
    public static List<IEntity> GetEntities()
    {
        if(LockEntities is null)
            LockEntities = new object();

        lock(LockEntities)
        {
            return Entities;
        }
    }
    public static void AddEntities(IEntity entity)
    {
        if(LockEntities is null)
            LockEntities = new object();

        lock(LockEntities)
        {
         Entities.Add(entity);
        }
    }
    public static ConcurrentBag<Player> Players { get; set; } = new ConcurrentBag<Player>();
    public static List<IEvent> Events {get; set; } = new List<IEvent>();
    static bool generated = false;
    public static Wall[] GenerateMap()
    {
        if(generated)
            return GetEntities().Where(e => e is Wall).Cast<Wall>().ToArray();
            
        Map =                   [[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
                                 [1,0,0,4,0,4,4,4,4,0,4,0,4,4,0,0,1],
                                 [1,0,4,4,4,0,0,4,0,0,4,0,0,4,4,0,1],
                                 [1,4,4,4,4,0,0,4,4,4,4,0,0,4,4,4,1],
                                 [1,4,0,4,4,4,4,4,4,4,0,0,0,0,4,4,1],
                                 [1,4,0,4,0,0,4,0,0,0,0,0,0,0,0,4,1],
                                 [1,4,4,4,4,4,4,4,4,4,4,0,0,0,0,4,1],
                                 [1,4,4,4,4,4,4,0,4,0,0,0,0,0,0,4,1],
                                 [1,4,4,4,0,0,4,0,4,0,0,0,0,0,0,4,1],
                                 [1,0,0,4,0,0,0,0,4,0,0,0,0,0,0,4,1],
                                 [1,4,4,4,0,0,0,4,4,4,4,4,4,0,0,4,1],
                                 [1,4,0,4,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,4,4,4,4,0,0,0,0,0,0,0,0,0,0,4,1],
                                 [1,0,4,4,4,0,0,0,0,0,0,0,4,4,4,0,1],
                                 [1,0,0,4,4,4,4,4,4,4,4,4,4,4,0,0,1],
                                 [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]];

        int X = 50;
        int Y = 50;
        
        foreach(var y in Map)
        {
            foreach(var x in y)
            {   
                if(x != 0)
                {
                    var wall = new Wall()
                    {
                        PosX = X,
                        PosY = Y,
                        Destructible = x != 1
                    };
                    GetEntities().Add(wall);
                }
                X += 50;
            }
            Y += 50;
            X = 50;
        }
        generated = true;
        return GetEntities().Where(e => e is Wall).Cast<Wall>().ToArray();
    }

    public static void AddPlayer(Player newPlayer) 
    { 
        Players.Add(newPlayer);
        GetEntities().Add(newPlayer);
    }
    
    public static void RemovePlayer(string Id)
    {
        var player = Players.FirstOrDefault(p => p.Id == Id);
        if (player != null)
            player.Live = false;
    }

    public static void MovePlayer(string id, MoveDirection moveDirection)
    {
        Thread.Sleep(5);
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
        {
            player.MovePlayer(moveDirection);
        }
    }

    public static void ChangeName(string id, string newName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
            player.Name = newName;
    }
    
    public static void PlantBomb(int x, int y) 
        => Bomb.Plant(x, y).Start();

    public static void RestartGame()
    {
        GetEntities().RemoveAll(e => e is Wall);
        generated = false;
        GenerateMap();

        var players = Players.ToArray();
        for (int i = 0; i < players.Length; i++)
        {
           if(i == 0)
           {
                players[0].PosX = 101; 
                players[0].PosY = 100;
           }
            else if(i == 1)
            {
                players[1].PosX = 99 + 14 * 50; 
                players[1].PosY = 100;
            }
            else if(i == 2)
            {
                players[2].PosX = 99 + 14 * 50; 
                players[2].PosY = 99 + 13 * 50;
            }
            else if(i == 3)
            {
                players[3].PosX = 101; 
                players[3].PosY = 99 + 13 * 50;
            }
        }
        
    }
}