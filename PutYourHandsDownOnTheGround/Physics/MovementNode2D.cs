using Godot;

namespace IsThisATiger2.Empty.Physics;

public partial class MovementNode2D : Node
{
    [Export(PropertyHint.Range, "0, 1.0")] private float _damping;
    [Export] private float _maxSpeed;
    [Export] private float _mass;

    private Vector2 _forces;

    private Vector2 _acceleration;
    private Vector2 _velocity;
    private Node2D _parent;

    public void AddForce(Vector2 force)
    {
        _forces += force;
    }

    public override void _Ready()
    {
        _parent = GetParent<Node2D>();
    }

    public override void _Process(double delta)
    {
        _acceleration = _forces * _mass;
        _forces = Vector2.Zero;

        _parent.Position += _velocity * (float)delta;
        _velocity = _acceleration * (float)delta;

        _velocity *= 1f - _damping;
        if(_maxSpeed > -1)
        {
            _velocity = _velocity.LimitLength(_maxSpeed);
        }
    }

    public void Stop()
    {
        _velocity = Vector2.Zero;
    }
}