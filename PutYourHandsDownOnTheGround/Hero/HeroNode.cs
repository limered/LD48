using Godot;
using IsThisATiger2.Empty.Physics;

namespace IsThisATiger2.Empty.Hero;

public partial class HeroNode : Node2D
{
    private MovementNode2D _movement;
    private Node2D _targetedTourist;
    private Vector2 _targetVector;

    public override void _Ready()
    {
        _movement = GetNode<MovementNode2D>("Movement");
    }

    public override void _Process(double delta)
    {
        var targetVec = _targetedTourist?.Position ?? _targetVector;
        if (targetVec.DistanceTo(Position) < 0.001) _movement.Direction = Vector2.Zero;
        _movement.Direction = Position.DirectionTo(targetVec);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            _targetVector = eventMouseButton.Position - GetViewport().GetVisibleRect().Size / 2;
        }
    }
}