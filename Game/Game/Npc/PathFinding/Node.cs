using Game.Game.Entities;

namespace Game.Game.Npc.PathFinding;

public class Node : IEqualityComparer<Node>
{
    public Node(Wall wall)
    {
        Wall = wall;
        Nodes.Add(this);
    }

    public int X { get; set; }
    public int Y { get; set; }
    private Wall Wall { get; }
    private static List<Node> Nodes { get; set; } = [];
    
    public IEnumerable<Node> Neighbours()
    {
        var onlyEmptySpaces = Nodes.Where(n => n.Wall.Empty || n.Wall.Destroyed);
        var asd1 = Nodes.Where(a => a.X == X && a.Y == Y + 1);
        var asd = onlyEmptySpaces.Where(a => a.X == X && a.Y == Y+1);
        var toReturn = new List<Node>();
        foreach (var node in onlyEmptySpaces)
        {
            if(IsRightNeighbour(node))
                toReturn.Add(node);
            else if(IsLeftNeighbour(node))
                toReturn.Add(node);
            else if(IsUpperNeighbour(node))
                toReturn.Add(node);
            else if(IsLowerNeighbour(node))
                toReturn.Add(node);
        }
        //var neighbours = onlyEmptySpaces.Where(n => IsUpperNeighbour(n) || IsLowerNeighbour(n) || IsLeftNeighbour(n) || IsRightNeighbour(n));
        
        return toReturn;
        
        bool IsUpperNeighbour(Node node) => node.Y == Y + 1 && node.X == X;
        bool IsLowerNeighbour(Node node) => node.Y == Y - 1 && node.X == X;
        bool IsLeftNeighbour(Node node) => node.X == X - 1 && node.Y == Y;
        bool IsRightNeighbour(Node node) => node.X == X + 1 && node.Y == Y;
    }

    private bool Visited { get; set; }
    
    public static List<Node>? DepthFirstSearch(Node currentNode, Node targetNode)
    {
        try
        {
            currentNode.Visited = true;

            if (currentNode == targetNode)
            {
                return new List<Node> { currentNode };
            }

            foreach (var neighbour in currentNode.Neighbours())
            {
                if (!neighbour.Visited)
                {
                    var path = DepthFirstSearch(neighbour, targetNode);
                    if (path != null)
                    {
                        path.Insert(0, currentNode);
                        return path;
                    }
                }
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public bool Equals(Node? x, Node? y)
    {
        return y != null && x != null && x.X == y.X && x.Y == y.Y;
    }

    public int GetHashCode(Node obj)
    {
        return obj.Neighbours().GetHashCode();
    }
}

public static class NodeExt
{
    public static Target ToTarget(this Node node)
    {
        return new Target(node.X, node.Y);
    }
}