public class Bonus : IEntity
{
    public string Id { get; init; }
    public bool Destructible { get; set; } = false;
    public bool Collision { get; set; } = true;
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; } = 32;
    public int Height { get; set; } = 32;
    public bool Destroyed { get; set; }
    public int BonusType { get; set; }

    public Bonus()
    {
        Id = Guid.NewGuid().ToString();
        BonusType = new Random().Next(1, 4);
    }
    public bool CheckCollistion(IEntity entity)
    {
        if(entity.Destroyed)
            return false;

        var h1 = this;
        var h2 = entity;
        var coll = h1 != h2 && h1.PosX < h2.PosX + h2.Width &&
         h1.PosX + h1.Width > h2.PosX &&
         h1.PosY < h2.PosY + h2.Height &&
         h1.Height + h1.PosY > h2.PosY;
        
        if(coll && entity is Player)
        {
            var p = entity as Player;
            if(p != null && BonusType == 2)
                p.Speed += 2;
            Destroyed = true;
        }

        return coll;
    }
}