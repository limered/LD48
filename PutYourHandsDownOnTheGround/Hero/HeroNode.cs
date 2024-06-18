using Godot;
using IsThisATiger2.Empty.Physics;
using IsThisATiger2.Empty.Tourist;
using IsThisATiger2.Empty.Utils;

namespace IsThisATiger2.Empty.Hero;

public partial class HeroNode : Node2D
{
    private MovementNode2D _movement;
    private Node2D _targetedTourist;
    private Vector2 _targetVector;
    private bool _touristClicked;

    public override void _Ready()
    {
        _movement = GetNode<MovementNode2D>("Movement");
        GetNode<EventBus>("/root/EventBus").Register<TouristClickedEvent>(TrackTourist);
    }

    private void TrackTourist(TouristClickedEvent obj)
    {
        _targetedTourist = obj.Tourist;
        _touristClicked = true;
    }

    public override void _Process(double delta)
    {
        var targetVec = _targetedTourist?.Position ?? _targetVector;
        if (targetVec.DistanceTo(Position) < 2)
        {
            _movement.Stop();
            return;
        }
        _movement.AddForce(Position.DirectionTo(targetVec) * 10000);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            _targetVector = eventMouseButton.Position - GetViewport().GetVisibleRect().Size / 2;
            if(!_touristClicked)
            {
                _targetedTourist = null;
            }
            else
            {
                _touristClicked = false;
            }
        }
    }
}