namespace GameEngine.Interface;

public interface IWorld
{
    public void Destroy();
    IReadOnlyList<IEntity> GetEntities();
    IReadOnlyList<IEntity> GetEntities<T>();
    void AddEntity(IEntity entity);
    void AddEntities(IEnumerable<IEntity> entities);
    void RemoveEntity(IEntity entity);
    void RemoveEntities(IEnumerable<IEntity> entities);
    void RemoveEntities<T>() where T : IEntity;
    bool Live { get; set; }
    bool Pause { get; set; }
    string GroupName { get; set; }
    int Id { get; set; }
    void Update(TimeSpan delta);
}