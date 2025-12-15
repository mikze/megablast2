using Game.Game.Npc.PathFinding;
using Game.Map;

namespace ServerTests;

public class SimplePath
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SimplePathTest1()
    {
        var gameMock = new GameMock();
        var map = MapHandler.GenerateMap(gameMock);
        if (map != null) Assert.That(map, Has.Length.EqualTo(272));
        if (map == null) Assert.Fail();
        if (map == null) return;
        var a = map.Select(w => w.Node).First(n => n is { Y: 2, X: 9 });
        var b = map.Select(w => w.Node).First(n => n is { Y: 8, X: 12 });
        var c = Node.DepthFirstSearch(a, b).ToArray();

        var visitedNodes = new HashSet<Node>();
        foreach (var node in c)
            if (!visitedNodes.Add(node))
                Assert.Fail();
        
    }
}