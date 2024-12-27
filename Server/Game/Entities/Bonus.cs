using Server.Game.Interface;

namespace Server.Game.Entities;

public class Bonus : EntityBase
{
    public int BonusType { get; set; }

    public Bonus(Game game) : base(game)
    {
        Id = Guid.NewGuid().ToString();
        BonusType = new Random().Next(1, 5);
        Width = 32;
        Height = 32;
        Collision = true;
    }
    public override bool CheckCollision(IEntity entity)
    {
        if(entity.Destroyed)
            return false;
        
        var coll = base.CheckCollision(entity);

        if (!coll || entity is not Player player) return coll;
        
        switch (BonusType)
        {
            case 4:
                player.IncreaseFireSize();
                break;
            case 3:
                Console.WriteLine($"Collided with bonus 3. Add live bonus to player {player.Id}, {player.LifeAmount()} ");
                player.AddLife();
                break;
            case 2:
                player.Speed += 0.1;
                break;
            case 1:
                player.MaxBombs += 1;
                break;
        }

        Destroyed = true;

        return coll;
    }
}