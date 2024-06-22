using System;
using Godot;
using IsThisATiger2.Empty.Distraction;
using IsThisATiger2.Empty.Physics;
using IsThisATiger2.Empty.Utils;

namespace IsThisATiger2.Empty.Tourist;

public partial class TouristNode : Node2D
{
    private Vector2 _anchor;
    [Export] private TouristState _currentState = TouristState.Idle;

    private Vector2 _idlePosition;
    private float _idleRadius = 100;
    [Export] private float _idleSpeed;
    [Export] private TouristConfiguration _images;
    private MovementNode2D _movement;
    private RandomNumberGenerator _rnd;
    private double _timeSinceLastInterest;

    private DistractionNode _currentDistraction;
    private DistractedTourist _distractedTourist;

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
                if (ShouldPickInterest(delta))
                {
                    PickInterest();
                    _currentState = TouristState.ToAttraction;
                }
                break;
            case TouristState.PickingInterest:
                // unused
                break;
            case TouristState.Interacting:
                _movement.Stop();
                _distractedTourist.Start();
                break;
            case TouristState.ToIdle:
                break;
            case TouristState.ToLevel:
                break;
            case TouristState.ToAttraction:
                GoToAttraction();
                if(Position.DistanceTo(_currentDistraction.Position) < GameStatics.DistractionDistance)
                {
                    _currentState = TouristState.Interacting;
                }
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

    private void GoToAttraction()
    {
        var goToDirection = (_currentDistraction.Position - Position).Normalized();
        _movement.AddForce(goToDirection * _idleSpeed);
    }

    private void PickInterest()
    {
        _currentDistraction = GetNode<DistractionCollection>("/root/DistractionCollection")
            .RandomDistraction(_rnd);
        _distractedTourist = (DistractedTourist) _currentDistraction.Bubble.Instantiate();
        _distractedTourist.DistractionWaitTime = _currentDistraction.DistractionDuration;
        AddChild(_distractedTourist);
    }

    private bool ShouldPickInterest(double dt)
    {
        _timeSinceLastInterest += dt;
        return _currentState == TouristState.Idle && 
               _rnd.Randf() < GameStatics.InterestPickingProbability &&
               _timeSinceLastInterest > GameStatics.TimeBetweenInterests;
    }

    private void GoToIdlePosition()
    {
        if (Position.DistanceTo(_idlePosition) < 2)
        {
            _movement.Stop();
            return;
        }

        var goToDirection = (_idlePosition - Position).Normalized();
        _movement.AddForce(goToDirection * _idleSpeed);
    }

    private void OnIdleTimeTimeout()
    {
        if (_currentState != TouristState.Idle) return;

        var needsToStop = _rnd.RandfRange(0, 1) < 0.6;
        if (needsToStop)
            _idlePosition = Position;
        else
            _idlePosition = _anchor + _rnd.RandomPointOnUnitRadius() * _idleRadius;
    }

    private void OnArea2dInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            GetNode<EventBus>("/root/EventBus").Emit(new TouristClickedEvent { Tourist = this });
    }
}