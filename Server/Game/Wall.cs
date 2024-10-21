
public class Wall : IEntity
{
    public string Id { get; init; }
    public bool Destructible { get; set; } = false;
    public bool Collision { get; set; } = true;
    public double PosX { get; set; }
    public double PosY { get; set; }
    public int Width { get; set; } = 33;
    public int Height { get; set; } = 33;
    public bool Destroyed { get; set; }

    public Wall()
    {
        Id = Guid.NewGuid().ToString();
    }
    public bool CheckCollistion(IEntity entity)
    {
        if(entity.Destroyed)
            return false;

        var h1 = this;
        var h2 = entity;
        return h1 != h2 && h1.PosX < h2.PosX + h2.Width &&
         h1.PosX + h1.Width > h2.PosX &&
         h1.PosY < h2.PosY + h2.Height &&
         h1.Height + h1.PosY > h2.PosY;


    }
}