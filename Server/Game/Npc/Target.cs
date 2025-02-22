namespace Server.Game.Npc;

public class Target
{
    public (double posY, double posX) Dest { get; set; }
    public bool Xdone { get; set; }
    public bool Ydone { get; set; }
    
    public bool Active => !Xdone || !Ydone;
}