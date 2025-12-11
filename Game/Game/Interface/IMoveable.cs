using GameEngine.Interface;

namespace Game.Game.Interface;

public interface IMoveable : IEntity
{
    public void Move(MoveDirection direction);
}