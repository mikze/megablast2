public class PlayerModel
{
    public PlayerModel(Player p)
    {
        PosX = p.PosX;
        PosY = p.PosY;
        Id = p.Id;
    }

    public int PosX { get; set; }
    public int PosY { get; set; }
    public string Id { get; set; }
}