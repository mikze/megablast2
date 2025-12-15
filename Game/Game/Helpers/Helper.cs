namespace Game.Game.Helpers;

public class Helper
{
    public static (int X, int Y) ToPixelsFromTiles(int tx, int ty, int tileSize = 50)
        => (tx * tileSize, ty * tileSize);
    
    public static (int X, int Y) ToTilesFromPixels(double tx, double ty, int tileSize = 50)
        => ((int)tx / tileSize, (int)ty / tileSize);
}