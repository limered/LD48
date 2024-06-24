using Godot;

namespace IsThisATiger2.Empty.Physics;

public partial class JumpyRunningAnimation : Node2D
{
    private double _time;
    private Vector2 _wobbleAxis = Vector2.Up;
    
    [Export] public MovementNode2D Movement;
    [Export] public float WobbleFactor = 0.1f;
    [Export] public float WobbleInterval = 0.04f;

    public override void _Process(double delta)
    {
        _time += delta;
        var speed = Movement.Velocity.Length();
        var a = Mathf.Sin(_time / WobbleInterval) * speed * WobbleFactor;
        a = Mathf.Abs(a) + a;
        Position = _wobbleAxis * (float)a;
    }
}