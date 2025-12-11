using Game.Game.Entities;

namespace Game.Game.Interface;

public interface IBullet : IMoveable
{
    public Player Owner { get; set; }
}