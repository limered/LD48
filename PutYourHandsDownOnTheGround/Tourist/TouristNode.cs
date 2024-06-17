using System;
using Godot;
using IsThisATiger2.Empty.Physics;
using IsThisATiger2.Empty.Utils;

namespace IsThisATiger2.Empty.Tourist;

public partial class TouristNode : Node2D
{
    private TouristState _currentState = TouristState.Idle;
    private Vector2 _goToPosition;
    [Export] private TouristConfiguration _images;
    private MovementNode2D _movement;
    private RandomNumberGenerator _rnd;

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

    private void OnIdleTimeTimeout()
    {
        if (_currentState != TouristState.Idle) return;

        var stop = _rnd.Randf();
        if (stop < 0.1)
        {
            // Stop
            _movement.Direction = Vector2.Zero;
        }
        else
        {
            // Move
            var pos = _goToPosition + _rnd.RandomPointOnUnitRadius();
            var rndMovement = (pos - Position).Rotated(_rnd.RandfRange(-90, 90)).Normalized();
            var delta = _movement.Direction + rndMovement;
            _movement.Direction = delta.Normalized();
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