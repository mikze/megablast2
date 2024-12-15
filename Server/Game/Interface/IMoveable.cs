namespace Server.Game.Interface;

public interface IMoveable : IEntity
{
    public void Move(MoveDirection direction);
}