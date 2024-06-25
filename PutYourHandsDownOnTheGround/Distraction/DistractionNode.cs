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
	private AnimationPlayer _player;

	public override void _Ready()
	{
		DistractionCollection.Add(this);
		_player = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		_player.SpeedScale = (float)GD.RandRange(0.85, 1.15);
		_player.Play(IdleAnimation);

		var area = GetNode<Area2D>("Area2D");
		area.AreaEntered += WakeUp;
		area.AreaExited += Sleep;
	}

	private void Sleep(Area2D area)
	{
		_player.PlayBackwards(WakeAnimation);
		_player.Play(IdleAnimation, 1D);
	}

	private void WakeUp(Area2D area)
	{
		_player.Play(WakeAnimation);
	}

	public void Kill()
	{
		_player.Play(KillAnimation);
		_player.Play(IdleAnimation, 0.8D);
	}
}
