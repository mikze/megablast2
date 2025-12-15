using System.Reflection;
using Game.Game.Entities;
using Game.Game.Npc.PathFinding;

namespace ServerTests;

class TileToPixTest
{
    [Test]
    public void Target_Should_Map_Tiles_To_World_With_PosY_PosX_Order()
    {
        // Arrange: tile X=6, tile Y=11 → world X=300, world Y=550 if tile size = 50
        var t = new Game.Game.Npc.Target(6, 11);

        // Act
        var (posY, posX) = t.Dest;

        // Assert
        Assert.That(posX, Is.EqualTo(6 * 50));
        Assert.That(posY, Is.EqualTo(11 * 50));
    }
    
    [Test]
    public void CreateDest_Should_Enqueue_Target_When_Called()
    {
        var dummyComm = NSubstitute.Substitute.For<Game.Game.Interface.ICommunicateHandler>();
        var game = new Game.Game.Game(dummyComm, "test");

        // Ensure we have a map and NPC
        _ = game.RestartGame(); // or call methods that generate the map; adapt to your API
        game.AddNpcPlayer();

        // Access the NPC (you may expose a helper in Game to get Npcs[0])
        var npcField = typeof(Game.Game.Game).GetField("Npcs", BindingFlags.NonPublic | BindingFlags.Instance);
        var npcs = game.Npcs;
        var npc = npcs[0];

        // Act: pick any plausible tile (replace with known empty tile if needed)
        npc.CreateDest(11, 6);

        // Assert
        Assert.That(npc.DestinationTargets.Count, Is.GreaterThan(0));
        Assert.That(npc.DestinationTargets[0].Active, Is.True);
    }
    
    [Test]
    public void Dfs_Finds_Straight_Path()
    {
        // Build a 3x1 line: (0,0)->(1,0)->(2,0) with all empty
        var nodes = new List<Node>();
        for (int x = 0; x < 3; x++)
        {
            var n = new Node { X = x, Y = 0, IsWalkable = true}; // or use a ctor that sets X,Y
            nodes.Add(n);
        }
        var path = Node.DepthFirstSearch(nodes[0], nodes[2]);
        Assert.That(path, Is.Not.Null);
        Assert.That(path!.Select(n => n.X), Is.EqualTo(new[] {0,1,2}));
    }
}