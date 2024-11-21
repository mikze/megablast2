namespace Server.Game.Interface;

public interface IMonster : IEntity
{
    public void Move(MoveDirection direction);
}