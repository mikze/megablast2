public interface IEntity
{
    public string Id { get; init; }
    public bool Collision { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool CheckCollistion(IEntity entity);
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; }
}