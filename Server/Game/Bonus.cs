public class Bonus : IEntity
{
    public string Id { get; init; }
    public bool Destructible { get; set; } = false;
    public bool Collision { get; set; } = true;
    public double PosX { get; set; }
    public double PosY { get; set; }
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
            if(p != null && BonusType == 3)
                p.Lives += 1;
            if(p != null && BonusType == 2)
                p.Speed += 0.1;
            if (p != null && BonusType == 1)
                p.MaxBombs += 1;
            Destroyed = true;
        }

        return coll;
    }
}