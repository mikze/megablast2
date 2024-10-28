
namespace Server.Game.Entities;

public class Wall : EntityBase
{
    public Wall()
    {
        Id = Guid.NewGuid().ToString();
        Collision = true;
        Width = Height = 33;
    }
}