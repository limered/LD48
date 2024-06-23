using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionNode : Node2D
{
	[Export] public float DistractionDuration;
	[Export] public string IdleAnimation;
	[Export] public string WakeAnimation;
	[Export] public string KillAnimation;
	[Export] public Texture2D WaitingTimeBubble;
	[Export] public Texture2D SpecialBubble;
	[Export] public bool IsDeadly;
	[Export] public Color BubbleColor;

	public override void _Ready()
	{
		GetNode<DistractionCollection>("/root/DistractionCollection").Add(this);
		var player = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		player.SpeedScale = (float)GD.RandRange(0.85, 1.15);
		player.Play(IdleAnimation);
	}
}
