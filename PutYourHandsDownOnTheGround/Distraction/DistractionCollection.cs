using System.Linq;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionCollection : Node
{
    [Export] private DistractionNode[] _distractions;

    public override void _Ready()
    {
        _distractions = GetTree()
            .GetNodesInGroup("distraction")
            .Cast<DistractionNode>()
            .ToArray();
    }

    public DistractionNode RandomDistraction(RandomNumberGenerator rnd)
    {
        return _distractions[rnd.RandiRange(0, _distractions.Length-1)];
    }
}