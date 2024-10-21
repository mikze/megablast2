
public class Player : IEntity
{
    public bool Dead { get; set; }
    public double Speed { get; set; } = 1;
    public string? Name { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public required string Id { get; init; }
    public bool Collision { get; set; } = true;
    public int Width { get; set; } = 36;
    public int Height { get; set; } = 43;
    public bool Moved { get; set; }
    public MoveDirection MoveDirection { get; set; }
    public bool Live { get; internal set; } = true;
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; } = true;
    public bool Ready { get; set; }
    public string Skin { get; set; } = "playerSprite";
    public int MaxBombs { get; set; } = 1;
    public int Lives { get; set; } = 1;

    public bool CheckCollistion(IEntity entity)
    {
        if(entity.Destroyed)
            return false;

        var h1 = this;
        var h2 = entity;
        var coll = h1 != h2 && h1.PosX < h2.PosX + h2.Width &&
         h1.PosX + h1.Width > h2.PosX &&
         h1.PosY < h2.PosY + h2.Height &&
         h1.Height + h1.PosY > h2.PosY;
        
        if(coll && entity is Bonus)
        {
            entity.Destroyed = true;
        }
        return coll;
    }

    public void MovePlayer(MoveDirection moveDirection)
    {
        if (!Dead && Game.Live)
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

            foreach (var entity in Game.GetEntities().Where(e => e != this && e.Collision))
            {
                if (entity.CheckCollistion(this))
                {
                    if (entity is Bomb && ((Bomb)entity).Owner == this)
                    {
                        var bomb = entity as Bomb;
                        if (bomb != null)
                        {
                            if(bomb.Touched)
                                break;
                        }
                    }

                    PosX = oldPosX;
                    PosY = oldPosY;
                    break;
                }
                else
                {
                    if (entity is Bomb && ((Bomb)entity).Owner == this)
                    {
                        var bomb = entity as Bomb;
                        if(bomb != null)
                            bomb.Touched = false;
                    }
                }
            }
        }
    }

    internal Bomb? PlantBomb()
    {
        if(Game.Entities.Where(e => e is Bomb).Where(e => (e as Bomb).Owner == this && !e.Destroyed).Count() < MaxBombs)
        if (!Dead)
            return Game.PlantBomb(PosX + 16, PosY + 16, this);

        return null;
    }
}