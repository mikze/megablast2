using GameEngine.Interface;

namespace GameEngine.Core;

public abstract class EntityBase(IWorld game) : IEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public bool Collision { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; }
    public IWorld Game { get; } = game;
    
    public virtual void Update()
    {
        
    }

    public virtual bool  CheckCollision(IEntity entity)
    {
        if (entity.Destroyed)
            return false;

        return this != entity && PosX < entity.PosX + entity.Width &&
               PosX + Width > entity.PosX &&
               PosY < entity.PosY + entity.Height &&
               Height + PosY > entity.PosY;
    }
}