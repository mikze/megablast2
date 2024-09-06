public class Wall : IEntity
{
    public bool Destructible { get; set; } = false;
    public bool Collision { get; set; } = true;
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; } = 50;
    public int Height { get; set; } = 50;

    public bool CheckCollistion(IEntity entity)
    {
        var h1 = this;
        var h2 = entity;
        return h1 != h2 && h1.PosX < h2.PosX + h2.Width &&
         h1.PosX + h1.Width > h2.PosX &&
         h1.PosY < h2.PosY + h2.Height &&
         h1.Height + h1.PosY > h2.PosY;


    }
}