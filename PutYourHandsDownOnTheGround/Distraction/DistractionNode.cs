using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionNode : Node2D
{
	[Export] public float DistractionDuration;
	[Export] public string IdleAnimation;
	[Export] public string KillAnimation;
	[Export] public Texture2D WaitingTimeBubble;
	[Export] public Texture2D SpecialBubble;
	[Export] public bool isDeadly;
	public PackedScene Bubble => GD.Load<PackedScene>("res://Scenes/ThinkingBubble.tscn");

	public override void _Ready()
	{
		GetNode<DistractionCollection>("/root/DistractionCollection").Add(this);
		GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer").Play(IdleAnimation);
	}

	public override void _Process(double delta)
	{
	}
}
