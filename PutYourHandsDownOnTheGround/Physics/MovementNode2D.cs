using Godot;

namespace IsThisATiger2.Empty.Physics;

public partial class MovementNode2D : Node
{
    [Export] private float _speed;
    [Export(PropertyHint.Range, "0, 1.0")] private float _damping;
    [Export] private float _maxSpeed;

    public Vector2 Direction { get; set; }

    private Vector2 _acceleration;
    private Vector2 _velocity;
    private Node2D _parent;

    public override void _Ready()
    {
        _parent = GetParent<Node2D>();
    }

    public override void _Process(double delta)
    {
        // apply direction (force)
        _acceleration += Direction * _speed;
        
        // apply damping
        var tempVelocity = _velocity + _acceleration * (float)delta;
        _parent.Position = (tempVelocity + _velocity) * 0.5f * (float)delta;
        _velocity = tempVelocity * _damping;

        _velocity = _velocity.LimitLength(_maxSpeed);
    }
}