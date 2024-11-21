using Server.Game.Entities;

namespace Server.Game;

public class MonsterFactory(int monsterAmount)
{
    internal int MonsterAmount { get; set; } = monsterAmount;

    public void GenerateMonsters()
    {
        Game.RemoveEntities(typeof(BasicMonster));
        Game.RemoveEntities(typeof(GhostMonster));
        var coords = Game.FindAllEmptyCoordinates().ToArray();
        Console.WriteLine($"Found {coords.Length} free spots");
        var rnd = new Random();
        var ghostMonster = MonsterAmount / 5;
        var normalMonsters = MonsterAmount - ghostMonster;
        
        for (var i = 0; i < normalMonsters; i++)
        {
            var r = rnd.Next(coords.Length-1);
            Game.AddEntities( new BasicMonster() { PosX = coords[r].X, PosY = coords[r].Y });
        }
        
        for (var i = 0; i < ghostMonster; i++)
        {
            var r = rnd.Next(coords.Length-1);
            Game.AddEntities( new GhostMonster() { PosX = coords[r].X, PosY = coords[r].Y });
        }
    }
}