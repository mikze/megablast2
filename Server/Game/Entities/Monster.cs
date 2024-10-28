namespace Server.Game.Entities;

public class Monster : EntityBase
{
    public Monster()
    {
        Id = Guid.NewGuid().ToString();
        Width = 36;
        Height = 43;
        Destructible = true;
    }
}