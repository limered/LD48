using Godot;
using IsThisATiger2.Empty.Tourist;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionManager : Node
{
	[Export] private DistractionNode[] _distractions;
	[Export] private TouristNode[] _tourists;

	private double _timer = 2;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		_timer -= delta;
		if (_timer <= 0)
		{
			var tourist = _tourists[0];
			var distraction = _distractions[0];

			var scene = GD.Load<PackedScene>("res://Scenes/ThinkingBubble.tscn");
			var instantiated = scene.Instantiate();
			tourist.AddChild(instantiated);
			
			_timer = 10;
		}
	}
}