public interface IEntity
{
    public bool Collision { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool CheckCollistion(IEntity entity);
}