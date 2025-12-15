using Game.Game.Entities;

namespace Game.Game.Npc.PathFinding;

public class Node : IEqualityComparer<Node>
{
    public Node()
    {
        Nodes.Add(this);
    }

    public int X { get; set; }
    public int Y { get; set; }
    public bool IsWalkable { get; set; }
    private static List<Node> Nodes { get; set; } = [];
    
    public IEnumerable<Node> Neighbours()
    {
        var onlyEmptySpaces = Nodes.Where(n => n.IsWalkable);
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
    
    public static List<Node>? DepthFirstSearch(Node start, Node target)
    {
        var visited = new HashSet<Node>();
        return Dfs(start, target, visited);

        static List<Node>? Dfs(Node current, Node target, HashSet<Node> visited)
        {
            if (!visited.Add(current)) return null; // already visited
            if (current.X == target.X && current.Y == target.Y)
                return new List<Node> { current };

            foreach (var nb in current.Neighbours())
            {
                var path = Dfs(nb, target, visited);
                if (path != null)
                {
                    path.Insert(0, current);
                    return path;
                }
            }
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