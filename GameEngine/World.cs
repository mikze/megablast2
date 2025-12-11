using GameEngine.Interface;

namespace GameEngine;

public abstract class World : IWorld
{
    public int Id { get; set; }
    public bool Live { get; set; } = true;
    public bool Pause { get; set; } = false;
    public string GroupName { get; set; }
    protected List<IEntity> _entities = [];
    private readonly object _lockObject = new ();
    
    public void RemoveEntities<T>() where T : IEntity
    {
        lock (_lockObject)
        {
            _entities.RemoveAll(x => x is T);
        }
    }
    
    public void AddEntities(IEnumerable<IEntity> entities)
    {
        lock (_lockObject)
        {
            _entities.AddRange(entities);
        }
    }

    public void RemoveEntity(IEntity entity)
    {
        lock (_lockObject)
        {
            _entities.Remove(entity);
        }
    }

    public void RemoveEntities(IEnumerable<IEntity> entities)
    {
        lock (_lockObject)
        {
            foreach (var entity in entities)
            {
                _entities.Remove(entity);
            }
        }
    }

    public void Destroy()
    {
        lock (_lockObject)
        {
            _entities.Clear();
        }
    }

    public IReadOnlyList<IEntity> GetEntities()
    {
        lock (_lockObject)
        {
            return _entities.ToArray();
        }
    }
    
    public IReadOnlyList<IEntity> GetEntities<T>()
    {
        lock (_lockObject)
        {
            return _entities.OfType<T>().Cast<IEntity>().ToArray();
        }
    }

    public void AddEntity(IEntity entity)
    {
        lock (_lockObject)
        {
            _entities.Add(entity);
        }
    }
    public event Action<TimeSpan>? OnUpdate; 
    public void Update(TimeSpan delta)
    {
        if(!Live) return;
        OnUpdate?.Invoke(delta);
        lock (_lockObject)
        {
            var entities = _entities.ToArray();
            foreach (var entity in entities)
            {
                if (entity.Destroyed) continue;
                entity.Update();
            }

            _entities.RemoveAll(e => e.Destroyed);
        }
    }
}