using Godot;

namespace IsThisATiger2.Empty.Physics;

public partial class MovementNode2D : Node
{
    private Vector2 _acceleration;
    private Vector2 _forces;
    private Node2D _parent;
    
    [Export(PropertyHint.Range, "0, 1.0")] public float Damping;
    [Export] public float Mass;
    [Export] public float MaxSpeed;
    public Vector2 Velocity;

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
        _acceleration = _forces * Mass;
        _forces = Vector2.Zero;

        _parent.Position += Velocity * (float)delta;
        Velocity = _acceleration * (float)delta;

        Velocity *= 1f - Damping;
        if (MaxSpeed > -1) Velocity = Velocity.LimitLength(MaxSpeed);
    }

    public void Stop()
    {
        Velocity = Vector2.Zero;
    }
}