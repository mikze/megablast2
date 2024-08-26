using System.Collections.Concurrent;
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
    public static int[][] Map;
    public static List<IEntity> Entities = new List<IEntity>();
    public static ConcurrentBag<Player> Players { get; set; } = new ConcurrentBag<Player>();
    public static List<IEvent> Events {get; set; } = new List<IEvent>();

    public static void GenerateMap()
    {
        Map =                   [[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                                 [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
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
                        PosY = Y
                    };
                    Entities.Add(wall);
                }
                X += 50;
            }
            Y += 50;
            X = 50;
        }
    }

    public static void AddPlayer(Player newPlayer) 
    { 
        Players.Add(newPlayer);
        Entities.Add(newPlayer);
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

    public static void AddEvent(IEvent @event)
    {
        try
        {
            Thread.Sleep(10);
        if(@event is IMovePlayer)
        {
            var a = (IMovePlayer)@event;
            if(Events.Cast<IMovePlayer>().Where(x => x.Player == a.Player).Count() > 50)
                return;
        }
        
        Events.Add(@event);
        }
        catch{}
    }
}