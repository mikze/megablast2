using Server.Game.Interface;

namespace Server.Game.Entities;

public class GhostMonster : BasicMonster
{
    private readonly Random _rnd = new ();
    private double _acc = 0;
    protected override double MovementIncrement => Game.MonsterSpeed - 0.5 + _acc;

    protected override bool CollisionCheck()
    {
        foreach (var entity in Game.GetEntities().Where(e => e.Id != Id))
            if (entity.CheckCollision(this))
            {
                if (entity is Player { Dead: false } player)
                    player.TakeLife();
                switch (entity)
                {
                    case Player { Dead: true }:
                    case Wall when entity.Destructible:
                        continue;
                    default:
                        return true;
                }
            }

        return false;
    }

    public override void Move(MoveDirection direction)
    {
        var oldPos = new { x = PosX, y = PosY };
        int r = _rnd.Next(1, 115);
        if (r == 2 && _acc == 0)
            _acc+=4.5;
        if(r > 102 && _acc > 0)
            _acc = 0;
        
        UpdatePosition(MoveDirection);
        if (r != 99 && !CollisionCheck()) return;
        PosX = oldPos.x;
        PosY = oldPos.y;
        ChangeDirection();
    }
}