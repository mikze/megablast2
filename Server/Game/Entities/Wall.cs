
namespace Server.Game.Entities;

public class Wall : EntityBase
{
    public Wall(Game game) : base(game)
    {
        Id = Guid.NewGuid().ToString();
        Collision = true;
        Width = Height = 45;
    }
}