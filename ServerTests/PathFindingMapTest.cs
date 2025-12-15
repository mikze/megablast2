using System.Reflection;
using Game.Game.Npc.PathFinding;
using NUnit.Framework; // <-- add (for TestContext)

namespace ServerTests;

public class PathFindingMapTest
{
    [Test]
    public void Dfs_Finds_Path_From_15_1_To_8_5_On_Given_Map()
    {
        // Reset Node's static registry to isolate from other tests
        var nodesProp = typeof(Node).GetProperty("Nodes", BindingFlags.NonPublic | BindingFlags.Static);
        nodesProp!.SetValue(null, new List<Node>());

        // Given map: 1 & 4 are walls; 0 & 9 are walkable
        int[][] map =
        [
            [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,9,9,4,0,4,4,4,4,0,0,0,0,0,9,9,1],
            [1,9,4,4,4,0,0,4,0,0,4,0,0,0,0,9,1],
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
            [1,9,4,4,4,0,0,0,0,0,0,0,4,4,4,9,1],
            [1,9,9,4,4,4,4,4,4,4,4,4,4,4,9,9,1],
            [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
        ];

        // Build nodes for all tiles
        var grid = new Node[17, 16];
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                var n = new Node { X = x, Y = y, IsWalkable = map[y][x] == 0 || map[y][x] == 9 };
                grid[x, y] = n;
            }
        }

        var start = grid[15, 1]; // (15,1)
        var goal = grid[8, 5];   // (8,5)

        Assert.That(start.IsWalkable, Is.True, "Start must be walkable");
        Assert.That(goal.IsWalkable, Is.True, "Goal must be walkable");

        // Act
        var path = Node.DepthFirstSearch(start, goal);

        // Assert
        Assert.That(path, Is.Not.Null, "Path should be found");
        Assert.That(path!.Count, Is.GreaterThan(0));
        Assert.That((path.First().X, path.First().Y), Is.EqualTo((start.X, start.Y)));
        Assert.That((path.Last().X, path.Last().Y), Is.EqualTo((goal.X, goal.Y)));

        TestContext.Progress.WriteLine($"Path length: {path.Count}");
        TestContext.Progress.WriteLine($"Start: ({start.X},{start.Y})  Goal: ({goal.X},{goal.Y})");
        TestContext.Progress.WriteLine("Steps:");

        // Validate that each step is on walkable and uses 4-connectivity
        for (int i = 1; i < path.Count; i++)
        {
            var prev = path[i - 1];
            var cur = path[i];

            Assert.That(cur.IsWalkable, Is.True, $"Node ({cur.X},{cur.Y}) must be walkable");

            var dx = cur.X - prev.X;
            var dy = cur.Y - prev.Y;
            var manhattan = Math.Abs(dx) + Math.Abs(dy);

            var dir =
                dx == 1 ? "Right" :
                dx == -1 ? "Left" :
                dy == 1 ? "Down" :
                dy == -1 ? "Up" :
                "Invalid";

            TestContext.Progress.WriteLine(
                $"  {i:00}: ({prev.X},{prev.Y}) -> ({cur.X},{cur.Y})  dir={dir}  dx={dx} dy={dy} manhattan={manhattan}");

            Assert.That(manhattan, Is.EqualTo(1), $"Steps must be adjacent (4-connected) at index {i}");
        }
    }
}