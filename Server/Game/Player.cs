
public class Player : IEntity
{
    public int Speed { get; set; } = 1;
    public string? Name { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public required string Id { get; init; }
    public bool Collision { get; set; } = true;
    public int Width { get; set; } = 50;
    public int Height { get; set; } = 50;
    public bool Moved { get; set; }
    public MoveDirection MoveDirection { get; set; }
    public bool Live { get; internal set; } = true;

    public bool CheckCollistion(IEntity entity)
    {
        return false;
    }

    public void MovePlayer(MoveDirection moveDirection)
    {
        var oldPosX = PosX;
        var oldPosY = PosY;

        switch (moveDirection)
        {
            case MoveDirection.Right:
                PosX += Speed;
                break;
            case MoveDirection.Left:
                PosX -= Speed;
                break;
            case MoveDirection.Up:
                PosY -= Speed;
                break;
            case MoveDirection.Down:
                PosY += Speed;
                break;
        }

        foreach(var entity in Game.Entities.Where(e => e != this && e.Collision))
        {
            if(entity.CheckCollistion(this))
            {
                PosX = oldPosX;
                PosY = oldPosY;
                break;
            }
        }
    }

    internal void PlantBomb()
    {
        Game.PlantBomb(PosX, PosY);
    }
}