using Game.Game.Entities;

namespace Game.Game.Models;

public class BombModel(Bomb p)
{
    public double PosX { get; set; } = p.PosX;
    public double PosY { get; set; } = p.PosY;
    public string Id { get; set; } = p.Id;
}