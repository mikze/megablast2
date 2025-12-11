using Game.Game.Npc.PathFinding;
using GameEngine.Core;
using GameEngine.Interface;

namespace Game.Game.Entities;

public class Wall : EntityBase
{
    private readonly bool _empty;
    public bool Empty  => Destroyed || _empty;
    public Node node { get; set; }
    public Wall(IWorld game, bool empty = false) : base(game)
    {
        Id = Guid.NewGuid().ToString();
        Collision = true;
        Width = Height = 45;
        _empty = empty;
    }
    public override bool CheckCollision(IEntity entity) => !_empty && base.CheckCollision(entity);
}