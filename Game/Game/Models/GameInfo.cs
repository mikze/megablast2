namespace Game.Game.Models;

public class GameInfo
{
    public string Name { get; set; }
    public int ActivePlayers { get; set; }
    public int MaxPlayers { get; set; }
    public bool PassRequired { get; set; }
}