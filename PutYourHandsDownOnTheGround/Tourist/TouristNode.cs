using System;
using Godot;
using IsThisATiger2.Empty.Physics;
using IsThisATiger2.Empty.Utils;

namespace IsThisATiger2.Empty.Tourist;

public partial class TouristNode : Node2D
{
    private TouristState _currentState = TouristState.Idle;
    private Vector2 _anchor;
    [Export] private TouristConfiguration _images;
    private MovementNode2D _movement;
    private RandomNumberGenerator _rnd;

    private Vector2 _idlePosition;
    private float _idleRadius = 100;
    [Export] private float _idleSpeed;

    public override void _Ready()
    {
        _rnd = new RandomNumberGenerator();
        _movement = GetNode<MovementNode2D>("Movement2D");
        GetNode<Sprite2D>("head").Texture = _images.Head;
        GetNode<Sprite2D>("Body").Texture = _images.Body;
    }

    public override void _Process(double delta)
    {
        switch (_currentState)
        {
            case TouristState.Idle:
                GoToIdlePosition();
                break;
            case TouristState.PickingInterest:
                break;
            case TouristState.Interacting:
                break;
            case TouristState.ToIdle:
                break;
            case TouristState.ToLevel:
                break;
            case TouristState.ToAttraction:
                break;
            case TouristState.ToIdleBack:
                break;
            case TouristState.ToExit:
                break;
            case TouristState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void GoToIdlePosition()
    {
        if(Position.DistanceTo(_idlePosition) < 2)
        {
            _movement.Stop();
            return;
        }
        var goToDirection = (_idlePosition - Position).Normalized();
        _movement.AddForce(goToDirection * 5000);
    }

    private void OnIdleTimeTimeout()
    {
        if (_currentState != TouristState.Idle) return;

        var needsToStop = _rnd.RandfRange(0, 1) < 0.6;
        if (needsToStop)
        {
            _idlePosition = Position;
        }
        else
        {
            _idlePosition = _anchor + _rnd.RandomPointOnUnitRadius() * _idleRadius;
        }
    }

    private void OnArea2dInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            GetNode<EventBus>("/root/EventBus").Emit(new TouristClickedEvent { Tourist = this });
        }
    }
}