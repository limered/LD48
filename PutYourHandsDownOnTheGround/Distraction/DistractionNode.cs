using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionNode : Node
{
	[Export] private float _distractionDuration;

	public override void _Ready()
	{
		// Add Distraction to Distraction List
	}

	public override void _Process(double delta)
	{
		// Instantiate Distraction to Tourist Connection Scene on Tourist
		// Tick Distraction To Tourist Connection
		// If Time is over, execute the action  
	}
}