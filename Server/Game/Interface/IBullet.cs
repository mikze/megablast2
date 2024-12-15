using Server.Game.Entities;

namespace Server.Game.Interface;

public interface IBullet : IMoveable
{
    public Player Owner { get; set; }
}