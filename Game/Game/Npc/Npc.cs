using Game.Game.Entities;
using Game.Game.Helpers;
using Game.Game.Npc.PathFinding;

namespace Game.Game.Npc;

public class ComputerPlayer
{
    private Player Player { get; set; }
    private Random RandomGenerator { get; set; }
    public List<Target> DestinationTargets { get; set; } = new List<Target>();

    // Constructor to initialize the ComputerPlayer with the associated Player object.
    public ComputerPlayer(Player player)
    {
        Player = player;
        RandomGenerator = new Random();
    }
    
    
    public void MoveToward()
    {
        var currentDestination = GetCurrentDestination();
        Console.WriteLine($"Current destination: {currentDestination}");
        if (currentDestination == null) return;
        
        if (!currentDestination.Xdone && Math.Abs((int)Player.PosX - (int)currentDestination.Dest.posX) != 0)
        {
            if (Player.PosX < currentDestination.Dest.posX)
                Player.MoveDirection = MoveDirection.Right;
            else if (Player.PosX > currentDestination.Dest.posX)
                Player.MoveDirection = MoveDirection.Left;
            if (Math.Abs((int)Player.PosX - (int)currentDestination.Dest.posX) < 3)
            {
                Console.WriteLine("Destination reached X");
                currentDestination.Xdone = true;
                Player.PosX = currentDestination.Dest.posX;
            }
        }
        else if(!currentDestination.Xdone)
        {
            Console.WriteLine("Destination reached X");
            currentDestination.Xdone = true;
        }
            
        if (!currentDestination.Ydone && Math.Abs((int)Player.PosY - (int)currentDestination.Dest.posY) != 0)
        {
            if (Player.PosY < currentDestination.Dest.posY)
                Player.MoveDirection = MoveDirection.Down;
            else if (Player.PosY > currentDestination.Dest.posY)
                Player.MoveDirection = MoveDirection.Up;
            if (Math.Abs((int)Player.PosY - (int)currentDestination.Dest.posY) < 3)
            {
                Console.WriteLine("Destination reached Y");
                currentDestination.Ydone = true;
                Player.PosY = currentDestination.Dest.posY;
            }
        }
        else if(!currentDestination.Ydone)
        {
            Console.WriteLine("Destination reached Y");
            currentDestination.Ydone = true;
        }

        if (currentDestination.Xdone && currentDestination.Ydone)
        {
            Console.WriteLine($"Final destination reached X: {Player.PosX} Y: {Player.PosY}");
            Player.MoveDirection = MoveDirection.None;
            RemoveDestination(currentDestination);
        }
    }

    void RemoveDestination(Target target)
        => DestinationTargets.Remove(target);

    Target? GetCurrentDestination()
        => DestinationTargets.FirstOrDefault(t => t.Active);
    

    // Check if the path to the destination is blocked
    public bool IsPathBlocked((double posY, double posX) destination, Game game)
    {
        // Basic logic: check if there’s an obstacle between the player and the destination
        // For simplicity, assume a straight line and use grid-based checks
        if (Math.Abs(Player.PosX - destination.posX) > 5)
        {
            int step = Player.PosX < destination.posX ? 1 : -1;
            for (double x = Player.PosX + step; Math.Abs(x - destination.posX) > 5; x += step)
            {
                if (game.IsObstacle(x, Player.PosY)) return true;
            }
        }
        else if (Math.Abs(Player.PosY - destination.posY) > 5)
        {
            var step = Player.PosY < destination.posY ? 1 : -1;
            for (double y = Player.PosY + step; Math.Abs(y - destination.posY) > 5; y += step)
            {
                if (game.IsObstacle(Player.PosX, y)) return true;
            }
        }

        return false; // No obstacle detected
    }
    
    

    // Plant a bomb if the path is blocked
    public void PlantBomb(Game game)
    {
        // Assuming the bomb is placed at the NPC's current position
        Player.PlantBomb();

        // Move to a safe position after planting the bomb (arbitrary safe position here)
        MoveToSafePosition(game);
    }

    // Find a safe position away from the bomb
    private void MoveToSafePosition(Game game)
    {
        // Basic logic: pick a random nearby position that is not in blast radius
        Console.WriteLine($"Player position is: {Player.PosX} {Player.PosY}");
        var safePositions = game.GetSafePositionsNear((Player.PosX, Player.PosY));
        Console.WriteLine($"Safe positions: {safePositions.Count}");
        if (safePositions.Count > 0)
        {
            var safePosition = safePositions[RandomGenerator.Next(safePositions.Count)];
            Player.MoveDirection = GetDirectionTowards(safePosition);
        }
    }

    // Convert a position into a move direction
    private MoveDirection GetDirectionTowards((double posX, double posY) target)
    {
        if (Player.PosX < target.posX) return MoveDirection.Right;
        if (Player.PosX > target.posX) return MoveDirection.Left;
        return Player.PosY < target.posY ? MoveDirection.Down : MoveDirection.Up;
    }

    // Update logic for the NPC
    public void Update()
    {
        //if (IsPathBlocked(destination, map))
      //  {
            //PlantBomb(map);
      //  }
        //else
       // {
            MoveToward();
      //  }
    }
    
    public void CreateDest(int y, int x)
    {
        Console.WriteLine($"Creating destination at {x},{y}");
        // Get all map walls/tiles; you likely want a node graph based on empty tiles
        var map = Player.Game.GetEntities<Wall>().Cast<Wall>();

        // Choose starting node based on current player grid position
        var startTileX = (int)Math.Round(Player.PosX / 50.0);
        var startTileY = (int)Math.Round(Player.PosY / 50.0);

        var currentNode = map.Where(w => w.Node.X == startTileX && w.Node.Y == startTileY)
            .Select(w => w.Node)
            .FirstOrDefault();

        var (dx, dy) = Helper.ToTilesFromPixels(x, y);
        var destinationNode = map.Select(w => w.Node)
            .FirstOrDefault(n => n.X == x && n.Y == y);

        if (currentNode == null || destinationNode == null)
            return;

        var nodes = Node.DepthFirstSearch(currentNode, destinationNode);
        if (nodes == null) return;

        DestinationTargets.Clear();
        foreach (var node in nodes)
            DestinationTargets.Add(node.ToTarget());
    }
}