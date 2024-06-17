using Godot;
using IsThisATiger2.Empty.Physics;

namespace IsThisATiger2.Empty.Hero;

public partial class HeroNode : Node2D
{
	private MovementNode2D _movement;
	private Vector2 _targetVector;
	private Node2D _targetedTourist;
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		var targetVec = _targetedTourist?.Position ?? _targetVector;
		_movement.Direction = (targetVec - Position).Normalized();
	}
}