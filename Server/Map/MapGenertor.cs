using Microsoft.AspNetCore.Mvc;
using Server.Game.Entities;

namespace Server.Map;

public static class MapHandler
{
    private static int[][]? _map;
    private static List<Wall>? _walls = [];

    private static List<(double X, double Y)> _emptySpaces = [];

    public static Wall[]? GenerateMap(Game.Game game)
    {
        _walls = new List<Wall>();
        _emptySpaces = new List<(double X, double Y)>();

        _map = [[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
                [1,9,9,4,0,4,4,4,4,0,4,0,4,4,9,9,1],
                [1,9,4,4,4,0,0,4,0,0,4,0,0,4,4,9,1],
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
                [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]];

        var posX = 50;
        var posY = 50;

        foreach (var y in _map)
        {
            foreach (var x in y)
            {
                if (x != 0 && x != 9)
                {
                    var wall = new Wall(game)
                    {
                        PosX = posX,
                        PosY = posY,
                        Destructible = x != 1
                    };
                    _walls?.Add(wall);
                }
                else
                {
                    if (x == 0)
                        _emptySpaces.Add((posX ,posY));
                }
                posX += 50;
            }
            posY += 50;
            posX = 50;
        }

        return _walls?.ToArray();
    }

    public static List<(double X, double Y)> GetEmptySpaces() => _emptySpaces;
    
}