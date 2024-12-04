namespace Server.Game.Interface;

public interface IGameObject
{
    string Id { get; init; }
    bool Collision { get; set; }
    double PosX { get; set; }
    double PosY { get; set; }
}

public interface IEntity : IGameObject
{
    public int Width { get; set; }
    public int Height { get; set; }
    public bool CheckCollision(IEntity entity);
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; }
    protected Game Game { get; }
}