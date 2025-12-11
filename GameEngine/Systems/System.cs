using GameEngine.Interface;

namespace GameEngine.Systems;

public abstract class GameSystem(IWorld world)
{
    public IWorld World { get; set; } = world;
    public abstract void Update(TimeSpan delta);
}

public abstract class EntitySystem<T>(IWorld world) : GameSystem(world)
{
    public override void Update(TimeSpan delta)
    {
        var targetEntities = World.GetEntities<T>();
        
        foreach (var entity in targetEntities)
            UpdateEntity(entity, delta);
    }
    
    protected abstract void UpdateEntity(IEntity entity, TimeSpan delta);
}