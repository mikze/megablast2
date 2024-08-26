public class MoveEvent : IMovePlayer
{
    public MoveEvent(Player player, MoveDirection moveDirection)
    {
        Player = player;
        Direction = moveDirection;
    }

    public Player Player { get; set; }
    public MoveDirection Direction { get; set; }
}