using System.Collections.Generic;
using System.Linq;
using Godot;
using IsThisATiger2.Empty.Tourist;

namespace IsThisATiger2.Empty.Game;

public partial class TouristGroup : Node2D
{
	private List<TouristNode> _touristsAlive;
	private List<TouristNode> _touristsDead;

	private PackedScene _touristScene = GD.Load<PackedScene>("res://Scenes/tourist.tscn");

	private bool _allTouristSpawned;
	
	public override void _Ready()
	{
		_touristsAlive = new List<TouristNode>(GameStatics.TouristStartCount);
		_touristsDead = new List<TouristNode>(GameStatics.TouristStartCount);
	}

	public override void _Process(double delta)
	{
		if (!_allTouristSpawned)
		{
			var spawnPosition = GD.RandRange(-300, 300);
			var idlePosition = new Vector2(GD.RandRange(-200, 200), GD.RandRange(-50, 50));
			var tourist = _touristScene.Instantiate<TouristNode>();
			tourist.LevelEntered(new Vector2(spawnPosition, Position.Y), idlePosition);
			
			var imageId = GD.RandRange(1, 5) + "." + GD.RandRange(1, 4);
			tourist.Name = "tourist" + imageId;
			while (_touristsAlive.Any(t => t.Name == tourist.Name))
			{
				imageId = GD.RandRange(1, 5) + "." + GD.RandRange(1, 4);
				tourist.Name = "tourist" + imageId;
			};

			tourist.Images = GD.Load<TouristConfiguration>($"res://Resources/Tourist/{imageId}.tres");
			_touristsAlive.Add(tourist);

			_allTouristSpawned = _touristsAlive.Count == GameStatics.TouristStartCount;
			
			AddChild(tourist);
		}
	}
}