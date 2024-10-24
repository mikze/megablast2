public static class MapHandler
{
    static int[][]? Map;
    static List<Wall>? Walls = new List<Wall>();

    public static Wall[]? GenerateMap()
    {
        Walls = new List<Wall>();

        Map = [[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
               [1,0,0,4,0,4,4,4,4,0,4,0,4,4,0,0,1],
               [1,0,4,4,4,0,0,4,0,0,4,0,0,4,4,0,1],
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
               [1,0,4,4,4,0,0,0,0,0,0,0,4,4,4,0,1],
               [1,0,0,4,4,4,4,4,4,4,4,4,4,4,0,0,1],
               [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]];

        int X = 50;
        int Y = 50;

        foreach (var y in Map)
        {
            foreach (var x in y)
            {
                if (x != 0)
                {
                    var wall = new Wall()
                    {
                        PosX = X,
                        PosY = Y,
                        Destructible = x != 1
                    };
                    Walls?.Add(wall);
                }
                X += 50;
            }
            Y += 50;
            X = 50;
        }

        return Walls?.ToArray();
    }
}