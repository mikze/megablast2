using Game.Map;
using NUnit.Framework.Internal;

namespace ServerTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var gameMock = new GameMock();
        var map = MapHandler.GenerateMap(gameMock);
        if (map != null) Assert.That(map, Has.Length.EqualTo(272));
        if (map == null) Assert.Fail();
        if (map == null) return;
        var a = map.Select(w => w.node).First(n => n is { Y: 1, X: 1 });
        Assert.That(a.Neighbours().Count(), Is.EqualTo(2));
        
        var b = map.Select(w => w.node).First(n => n is { Y: 1, X: 2 });
        Assert.That(b.Neighbours().Count(), Is.EqualTo(1));
        
        var c = map.Select(w => w.node).First(n => n is { Y: 2, X: 1 });
        Assert.That(c.Neighbours().Count(), Is.EqualTo(1));
        
        var d = map.Select(w => w.node).First(n => n is { Y: 1, X: 12 });
        Assert.That(d.Neighbours().Count(), Is.EqualTo(3));
        
    }
}