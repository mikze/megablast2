namespace Game.Game.Npc;

public class Target
{
    public Target(int posX, int posY)
    {
        Dest = (posY * 50, posX * 50);
    }
    public (double posY, double posX) Dest { get; set; }
    public bool Xdone { get; set; }
    public bool Ydone { get; set; }
    
    public bool Active => !Xdone || !Ydone;
}