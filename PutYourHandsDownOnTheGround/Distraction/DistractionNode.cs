using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionNode : Node2D
{
	[Export] public float DistractionDuration;
	public PackedScene Bubble => GD.Load<PackedScene>("res://Scenes/ThinkingBubble.tscn");

	public override void _Ready()
	{
		GetNode<DistractionCollection>("/root/DistractionCollection").Add(this);
		GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer")
			.Play("flutter");
	}

	public override void _Process(double delta)
	{
		// Instantiate Distraction to Tourist Connection Scene on Tourist
		// Tick Distraction To Tourist Connection
		// If Time is over, execute the action  
	}
}