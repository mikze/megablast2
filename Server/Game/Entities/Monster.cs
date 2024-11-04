// Monster.cs
namespace Server.Game.Entities;

public class Monster : EntityBase
{
    private MoveDirection MoveDirection { get; set; }

    public Monster()
    {
        Id = Guid.NewGuid().ToString();
        Width = 36;
        Height = 43;
        Destructible = true;
    }

    private const double MovementIncrement = 0.5;

    public void Move()
    {
        var oldPos = new { x = PosX, y = PosY };
        UpdatePosition(MoveDirection);
        if (!CollisionCheck()) return;
        PosX = oldPos.x;
        PosY = oldPos.y;
        ChangeDirection();
    }

    private bool CollisionCheck()
    {
        foreach (var entity in Game.GetEntities().Where(e => e.Id != Id))
            if (entity.CheckCollision(this))
            {
                if (entity is Player { Dead: false } player)
                    player.TakeLife();
                if (entity is Player { Dead: true })
                    continue;
                    
                return true;
            }

        return false;
    }

    private void ChangeDirection()
    {
        MoveDirection = GenerateRandomDirection();
    }

    private MoveDirection GenerateRandomDirection()
    {
        var directions = Enum.GetValues(typeof(MoveDirection)).Cast<MoveDirection>().ToArray();
        var random = new Random();
        return directions[random.Next(0, directions.Length-1)];
    }

    private void UpdatePosition(MoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case MoveDirection.Right:
                PosX += MovementIncrement;
                break;
            case MoveDirection.Left:
                PosX -= MovementIncrement;
                break;
            case MoveDirection.Up:
                PosY -= MovementIncrement;
                break;
            case MoveDirection.Down:
                PosY += MovementIncrement;
                break;
            case MoveDirection.None:
                PosY += MovementIncrement;
                break;
            default:
                PosY += MovementIncrement;
                break;
        }
    }
}