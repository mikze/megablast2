using GameEngine.Interface;

namespace GameEngine;

public abstract class World : IWorld
{
    public int Id { get; set; }
    public bool Live { get; set; } = true;
    public bool Pause { get; set; } = false;
    public string GroupName { get; set; }
    protected List<IEntity> Entities = [];
    private readonly object _lockObject = new ();
    
    public void RemoveEntities<T>() where T : IEntity
    {
        lock (_lockObject)
        {
            Entities.RemoveAll(x => x is T);
        }
    }
    
    public void AddEntities(IEnumerable<IEntity> entities)
    {
        lock (_lockObject)
        {
            Entities.AddRange(entities);
        }
    }

    public void RemoveEntity(IEntity entity)
    {
        lock (_lockObject)
        {
            Entities.Remove(entity);
        }
    }

    public void RemoveEntities(IEnumerable<IEntity> entities)
    {
        lock (_lockObject)
        {
            foreach (var entity in entities)
            {
                Entities.Remove(entity);
            }
        }
    }

    public void Destroy()
    {
        lock (_lockObject)
        {
            Entities.Clear();
        }
    }

    public IReadOnlyList<IEntity> GetEntities()
    {
        lock (_lockObject)
        {
            return Entities.ToArray();
        }
    }
    
    public IReadOnlyList<IEntity> GetEntities<T>()
    {
        lock (_lockObject)
        {
            return Entities.OfType<T>().Cast<IEntity>().ToArray();
        }
    }

    public void AddEntity(IEntity entity)
    {
        lock (_lockObject)
        {
            Entities.Add(entity);
        }
    }
    public event Action<TimeSpan>? OnUpdate; 
    public void Update(TimeSpan delta)
    {
        if(!Live) return;
        OnUpdate?.Invoke(delta);
        lock (_lockObject)
        {
            var entities = Entities.ToArray();
            foreach (var entity in entities)
            {
                if (entity.Destroyed) continue;
                entity.Update();
            }

            Entities.RemoveAll(e => e.Destroyed);
        }
    }
}