using GameEngine.Interface;

/*namespace GameEngine.Systems;

public class MovementSystem(IWorld world) : EntitySystem<IMoveable>(world)
{
    protected override void UpdateEntity(IEntity entity, TimeSpan delta)
    {
        public void Move(MoveDirection moveDirection)
        {
            if (Dead || !Game.Live) return;
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

            foreach (var entity in Game.GetEntities().Where(e => e != this && e.Collision))
            {
                if (entity.CheckCollision(this))
                {
                    if (entity is Bullet bullet && bullet.Owner == this)
                        break;
                
                    if (entity is Bomb bomb && bomb.Owner == this)
                        if(bomb.Touched)
                            break;

                    PosX = oldPosX;
                    PosY = oldPosY;
                    break;
                }

                if (entity is not Bomb bomb1 || bomb1.Owner != this) continue;
                bomb1.Touched = false;
            }
        }
    }
}*/