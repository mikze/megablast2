using Server.Game.Entities;

namespace Server.Game.Models;

public class PlayerModel(Player p)
{
    public double PosX { get; set; } = p.PosX;
    public double PosY { get; set; } = p.PosY;
    public string Id { get; set; } = p.Id;
    public int Lives { get; set; } = p.LifeAmount();
}