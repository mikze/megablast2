public interface IEvent
{

}

public interface IMovePlayer : IEvent
{
    public Player Player { get; set; }
    public MoveDirection Direction { get; set; }
}