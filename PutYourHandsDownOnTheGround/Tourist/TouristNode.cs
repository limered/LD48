using System;
using System.Linq;
using Godot;
using IsThisATiger2.Empty.Distraction;
using IsThisATiger2.Empty.Hero;
using IsThisATiger2.Empty.Physics;
using IsThisATiger2.Empty.Utils;

namespace IsThisATiger2.Empty.Tourist;

public partial class TouristNode : Node2D
{
    private const float IdleRadius = 100;
    private Vector2 _anchor;
    private Area2D _area2d;
    private PackedScene _bubble = GD.Load<PackedScene>("res://Scenes/ThinkingBubble.tscn");
    private DistractionNode _currentDistraction;
    private DistractedTourist _distractedTourist;
    private Vector2 _idlePosition;
    private MovementNode2D _movement;
    private RandomNumberGenerator _rnd;
    private Texture2D _skullImage = GD.Load<Texture2D>("res://Graphics/Bubbles/scull.png");
    private double _timeSinceLastInterest;

    [Export] public TouristState CurrentState = TouristState.Idle;
    [Export] public float IdleSpeed;
    [Export] public TouristConfiguration Images;

    public override void _Ready()
    {
        _rnd = new RandomNumberGenerator();
        _movement = GetNode<MovementNode2D>("Movement2D");
        GetNode<Sprite2D>("head").Texture = Images.Head;
        GetNode<Sprite2D>("Body").Texture = Images.Body;

        _area2d = GetNode<Area2D>("Area2D");
        _area2d.AreaEntered += OnArea2dBodyEntered;
        _area2d.AreaExited += OnArea2dBodyExited;
    }

    public override void _Process(double delta)
    {
        switch (CurrentState)
        {
            case TouristState.Idle:
                GoToIdlePosition();
                if (ShouldPickInterest(delta))
                {
                    PickInterest();
                    CurrentState = TouristState.ToAttraction;
                }

                break;
            case TouristState.PickingInterest:
                // unused
                break;
            case TouristState.Interacting:
                _movement.Stop();
                _distractedTourist.Start(
                    () =>
                    {
                        if (_currentDistraction.IsDeadly)
                        {
                            _currentDistraction.Kill();
                            CurrentState = TouristState.Dead;
                        }
                        else
                        {
                            CurrentState = TouristState.ToIdle;
                        }
                    },
                    _currentDistraction.BubbleColor);
                break;
            case TouristState.ToIdle:
            {
                var reached = GoToIdlePosition();
                if (reached)
                {
                    _timeSinceLastInterest = 0;
                    _distractedTourist.Free();
                    _currentDistraction = null;
                    _distractedTourist = null;
                    CurrentState = TouristState.Idle;
                }

                break;
            }
            case TouristState.ToLevel:
                break;
            case TouristState.ToAttraction:
                GoToAttraction();
                if (Position.DistanceTo(_currentDistraction.Position) < GameStatics.DistractionDistance)
                    CurrentState = TouristState.Interacting;
                break;
            case TouristState.ToIdleWithPlayer:
            {
                var reached = GoToIdlePosition();
                if (reached)
                {
                    _timeSinceLastInterest = 0;
                    _distractedTourist.Free();
                    _currentDistraction = null;
                    _distractedTourist = null;
                    CurrentState = TouristState.Idle;
                }

                break;
            }
            case TouristState.ToExit:
                break;
            case TouristState.Dead:
                _distractedTourist.SetImage(_skullImage);
                // show blood
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void GoToAttraction()
    {
        var goToDirection = (_currentDistraction.Position - Position).Normalized();
        _movement.AddForce(goToDirection * IdleSpeed);
    }

    private void PickInterest()
    {
        _currentDistraction = DistractionCollection.RandomDistraction(_rnd);
        _distractedTourist = (DistractedTourist)_bubble.Instantiate();
        _distractedTourist.SetImage(_currentDistraction.WaitingTimeBubble);
        _distractedTourist.DistractionWaitTime = _currentDistraction.DistractionDuration;

        AddChild(_distractedTourist);
    }

    private bool ShouldPickInterest(double dt)
    {
        _timeSinceLastInterest += dt;
        return CurrentState == TouristState.Idle &&
               _rnd.Randf() < GameStatics.InterestPickingProbability &&
               _timeSinceLastInterest > GameStatics.InterestCooldown &&
               !_area2d.GetOverlappingAreas().Any(a => a.Owner is HeroNode);
    }

    private bool GoToIdlePosition()
    {
        if (Position.DistanceTo(_idlePosition) < 2)
        {
            _movement.Stop();
            return true;
        }

        var goToDirection = (_idlePosition - Position).Normalized();
        _movement.AddForce(goToDirection * IdleSpeed);
        return false;
    }

    private void OnIdleTimeTimeout()
    {
        if (CurrentState != TouristState.Idle) return;

        var needsToStop = _rnd.RandfRange(0, 1) < 0.6;
        if (needsToStop)
            _idlePosition = Position;
        else
            _idlePosition = _anchor + _rnd.RandomPointOnUnitRadius() * IdleRadius;
    }

    private void OnArea2dInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            EventBus.Emit(new TouristClickedEvent { Tourist = this });
    }

    private void OnArea2dBodyEntered(Node2D other)
    {
        if (other.Owner is not HeroNode hero ||
            !hero.TargetsThisTourist(this) ||
            CurrentState is not (TouristState.ToAttraction or TouristState.Interacting)) return;

        _distractedTourist.Abort();
        CurrentState = TouristState.ToIdleWithPlayer;
    }

    private void OnArea2dBodyExited(Node2D other)
    {
        if (other.Owner is HeroNode &&
            CurrentState is TouristState.ToIdleWithPlayer)
            CurrentState = TouristState.ToAttraction;
    }
}