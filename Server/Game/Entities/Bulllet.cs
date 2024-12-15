using Server.Game.Interface;

namespace Server.Game.Entities;

public class Bullet : EntityBase, IBullet
{
    public Bullet(Game game, Player owner, double sinA, double cosA) : base(game)
    {
        Id = Guid.NewGuid().ToString();
        Width = 20;
        Height = 20;
        PosX = owner.PosX;
        PosY = owner.PosY;
        Collision = true;
        _cosA = cosA;
        _sinA = sinA;
        Owner = owner;
    }
    
    private readonly double _cosA;
    private readonly double _sinA;
    private int Speed { get; set; } = 3;
    public Player Owner { get; set; }
    public void Move(MoveDirection direction)
    {
        PosX += _cosA * Speed;
        PosY += _sinA * Speed;
        foreach (var entity in Game.GetEntities().Where(e => e != this && e.Collision))
        {
            if (!entity.CheckCollision(this)) continue;
            if(entity != Owner)
                Destroyed = true;
            

            if (entity != Owner && entity is Player player)
                player.TakeLife(1);
                
            break;
        }
    }

    public override bool CheckCollision(IEntity entity)
    {
        var ba= base.CheckCollision(entity);

        if (!ba) return ba;
        
        if (entity is Bullet bullet)
        {
            if (bullet.Owner != Owner)
                Destroyed = true;
        }
        else if(entity != Owner)
            Destroyed = true;


        return ba;
    }
}