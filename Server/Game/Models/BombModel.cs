public class BombModel
{
    public BombModel(Bomb p)
    {
        PosX = p.PosX;
        PosY = p.PosY;
        Id = p.Id;
    }

    public double PosX { get; set; }
    public double PosY { get; set; }
    public string Id { get; set; }
}