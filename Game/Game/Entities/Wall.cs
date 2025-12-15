using Game.Game.Helpers;
using Game.Game.Npc.PathFinding;
using GameEngine.Core;
using GameEngine.Interface;

namespace Game.Game.Entities;

public class Wall : EntityBase
{
    private readonly bool _empty;
    public bool Empty  => Destroyed || _empty;
    public Node Node => new() { IsWalkable = Empty };
    public (int x, int y) Coord => Helper.ToTilesFromPixels(PosX, PosY, 50);
    public Wall(IWorld game, bool empty = false) : base(game)
    {
        Id = Guid.NewGuid().ToString();
        Collision = true;
        Width = Height = 45;
        _empty = empty;
    }
    public override bool CheckCollision(IEntity entity) => !_empty && base.CheckCollision(entity);
}