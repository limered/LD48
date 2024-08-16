using Godot;

namespace IsThisATiger2.Empty.Plants;

public partial class BushNode : Node2D
{
	[Export] public bool FlipH;
	[Export] public Texture2D[] Bushes;
	[Export] public int BushSpriteId;
	
	public override void _Ready()
	{
		if (BushSpriteId > Bushes.Length) BushSpriteId = GD.RandRange(0, 2);
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = Bushes[BushSpriteId];
		sprite.FlipH = FlipH;

		var area = GetNode<Area2D>("Area2D");
		area.AreaEntered += OnAreaEntered;
		area.AreaExited += OnAreaEntered;

		area.MouseEntered += AreaOnMouseEntered;
	}

	private void AreaOnMouseEntered()
	{
		Animate();
	}

	private void OnAreaEntered(Area2D area)
	{
		Animate();
	}

	private void Animate()
	{
		var rnd = GD.RandRange(-0.3d, 0.3d);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "skew", rnd, 0.1f);
		tween.TweenProperty(this, "skew", 0f, 0.05f);
		tween.TweenProperty(this, "skew", -rnd, 0.1f);
		tween.TweenProperty(this, "skew", 0f, 0.05f);
	}
}