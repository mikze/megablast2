using Game.Map;
using NUnit.Framework.Internal;

namespace ServerTests;

public class MapFieldNumber
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var gameMock = new GameMock();
        gameMock.Entities.AddRange(MapHandler.GenerateMap(gameMock));
        var map = gameMock.GetAllMap();
        if (map != null) Assert.That(map, Has.Length.EqualTo(272));
        if (map == null) Assert.Fail();
        if (map == null) return;
        
        map = gameMock.GetAllMap();
        if (map != null) Assert.That(map, Has.Length.EqualTo(272));
        if (map == null) Assert.Fail();
        if (map == null) return;
    }
}