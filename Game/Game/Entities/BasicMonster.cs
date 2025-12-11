// Monster.cs

using Game.Game.Interface;
using GameEngine.Core;

namespace Game.Game.Entities;

public class BasicMonster : EntityBase, IMonster
{
    protected MoveDirection MoveDirection { get; set; }

    public BasicMonster(Game game) : base(game)
    {
        Id = Guid.NewGuid().ToString();
        Width = 36;
        Height = 43;
        Destructible = true;
    }

    protected virtual  double MovementIncrement => _speed;
    static double _speed = 1.3;

    public virtual void Move(MoveDirection direction)
    {
        var oldPos = new { x = PosX, y = PosY };
        UpdatePosition(MoveDirection);
        if (!CollisionCheck()) return;
        PosX = oldPos.x;
        PosY = oldPos.y;
        ChangeDirection();
    }

    protected virtual bool CollisionCheck()
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

    protected void ChangeDirection()
    {
        MoveDirection = GenerateRandomDirection();
    }

    private MoveDirection GenerateRandomDirection()
    {
        var directions = Enum.GetValues(typeof(MoveDirection)).Cast<MoveDirection>().ToArray();
        var random = new Random();
        return directions[random.Next(0, directions.Length-1)];
    }

    protected void UpdatePosition(MoveDirection moveDirection)
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
            case MoveDirection.None:
            default:
                PosY += MovementIncrement;
                break;
        }
    }
}