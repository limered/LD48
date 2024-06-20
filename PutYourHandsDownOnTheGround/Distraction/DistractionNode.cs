using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionNode : Node
{
	[Export] private float _distractionDuration;

	public override void _Ready()
	{
		GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer").Play("flutter");
	}

	public override void _Process(double delta)
	{
		// Instantiate Distraction to Tourist Connection Scene on Tourist
		// Tick Distraction To Tourist Connection
		// If Time is over, execute the action  
	}
}