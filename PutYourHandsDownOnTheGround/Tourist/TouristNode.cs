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
    private double _timeToLivePoisoned;

    [Export] public TouristState CurrentState = TouristState.Idle;
    [Export] public TouristConfiguration Images;
    
    public override void _Ready()
    {
        _rnd = new RandomNumberGenerator();
        _movement = GetNode<MovementNode2D>("Movement2D");
        GetNode<Sprite2D>("SpriteContainer/head").Texture = Images.Head;
        GetNode<Sprite2D>("SpriteContainer/Body").Texture = Images.Body;

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
                        if (_currentDistraction.IsSpider)
                        {
                            _timeToLivePoisoned = _currentDistraction.DistractionDuration;
                            CurrentState = TouristState.Poisoned;
                        }
                        else if (_currentDistraction.IsDeadly)
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
                    ResetToIdle();
                }

                break;
            }
            case TouristState.ToLevel:
            {
                var reached = GoToIdlePosition();
                if (reached)
                {
                    ResetToIdle();
                }
                break;
            }
            case TouristState.ToAttraction:
                GoToAttraction();
                if (GlobalPosition.DistanceTo(_currentDistraction.GlobalPosition) < GameStatics.DistractionDistance)
                    CurrentState = TouristState.Interacting;
                break;
            case TouristState.ToIdleWithPlayer:
            {
                var reached = GoToIdlePosition();
                if (reached)
                {
                    ResetToIdle();
                }

                break;
            }
            case TouristState.ToExit:
                break;
            case TouristState.Dead:
                _distractedTourist.SetImage(_skullImage);
                // show blood
                break;
            case TouristState.Poisoned:
                GoToIdlePosition();
                _timeToLivePoisoned -= delta;
                if (_timeToLivePoisoned <= 0)
                {
                    CurrentState = TouristState.Dead;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void LevelEntered(Vector2 startPosition, Vector2 idlePosition)
    {
        GlobalPosition = startPosition;
        _idlePosition = idlePosition;
        CurrentState = TouristState.ToLevel;
    }
    
    private void ResetToIdle()
    {
        _timeSinceLastInterest = 0;
        _distractedTourist?.Free();
        _currentDistraction = null;
        _distractedTourist = null;
        CurrentState = TouristState.Idle;
    }

    private void GoToAttraction()
    {
        var goToDirection = (_currentDistraction.GlobalPosition - GlobalPosition).Normalized();
        _movement.AddForce(goToDirection * GameStatics.TouristDistractionSpeed);
    }

    private void PickInterest()
    {
        _currentDistraction = DistractionCollection.RandomDistraction(_rnd);
        _distractedTourist = (DistractedTourist)_bubble.Instantiate();
        _distractedTourist.GlobalPosition = new Vector2(0, -160);
        _distractedTourist.Scale = new Vector2(0.7f, 0.7f);
        _distractedTourist.SetImage(_currentDistraction.WaitingTimeBubble);
        _distractedTourist.DistractionWaitTime = _currentDistraction.DistractionDuration;

        AddChild(_distractedTourist);
    }

    private bool ShouldPickInterest(double dt)
    {
        _timeSinceLastInterest += dt;
        return CurrentState == TouristState.Idle &&
               _rnd.Randf() < GameStatics.InterestPickingProbability &&
               _timeSinceLastInterest > _rnd.RandfRange(GameStatics.InterestCooldown.X, GameStatics.InterestCooldown.Y) &&
               !_area2d.GetOverlappingAreas().Any(a => a.Owner is HeroNode);
    }

    private bool GoToIdlePosition()
    {
        if (GlobalPosition.DistanceTo(_idlePosition) < 2)
        {
            _movement.Stop();
            return true;
        }

        var goToDirection = (_idlePosition - GlobalPosition).Normalized();
        _movement.AddForce(goToDirection * GameStatics.TouristIdleSpeed);
        return false;
    }

    private void OnIdleTimeTimeout()
    {
        if (CurrentState != TouristState.Idle) return;

        var needsToStop = _rnd.RandfRange(0, 1) < 0.6;
        if (needsToStop)
            _idlePosition = GlobalPosition;
        else
            _idlePosition = _anchor + _rnd.RandomPointOnUnitRadius() * IdleRadius;
    }

    private void OnArea2dInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            EventBus.Emit(new TouristClickedEvent { Tourist = this });
            if (_area2d.GetOverlappingAreas().Any(a => a.Owner is HeroNode) &&
                CurrentState is (TouristState.ToAttraction or TouristState.Interacting or TouristState.Poisoned))
            {
                _distractedTourist.Abort();
                CurrentState = TouristState.ToIdleWithPlayer;
            }
        }
    }

    private void OnArea2dBodyEntered(Node2D other)
    {
        if (other.Owner is HeroNode hero &&
            hero.TargetsThisTourist(this) &&
            CurrentState is TouristState.ToAttraction or TouristState.Interacting or TouristState.Poisoned)
        {
            _distractedTourist.Abort();
            CurrentState = TouristState.ToIdleWithPlayer;
        }
    }

    private void OnArea2dBodyExited(Node2D other)
    {
        if (other.Owner is HeroNode &&
            CurrentState is TouristState.ToIdleWithPlayer)
            CurrentState = TouristState.ToAttraction;
    }
}