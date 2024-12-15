using Server.Game.Entities;
using Server.Game.Interface;

namespace Server.Game;

public class MonsterFactory(int monsterAmount, Game game)
{
    internal int MonsterAmount { get; set; } = monsterAmount;

    public void GenerateMonsters()
    {
        //game.RemoveEntities(typeof(BasicMonster));
        //game.RemoveEntities(typeof(GhostMonster));
        game.RemoveEntities<IMoveable>();
        //game.RemoveEntities(null);
        var coords = game.FindAllEmptyCoordinates().ToArray();
        Console.WriteLine($"Found {coords.Length} free spots");
        var rnd = new Random();
        var ghostMonster = MonsterAmount / 5;
        var normalMonsters = MonsterAmount - ghostMonster;
        
        for (var i = 0; i < normalMonsters; i++)
        {
            var r = rnd.Next(coords.Length-1);
            game.AddEntities( new BasicMonster(game) { PosX = coords[r].X, PosY = coords[r].Y });
        }
        
        for (var i = 0; i < ghostMonster; i++)
        {
            var r = rnd.Next(coords.Length-1);
            game.AddEntities( new GhostMonster(game) { PosX = coords[r].X, PosY = coords[r].Y });
        }
    }
}