using System.Collections.Generic;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractionCollection : Node
{
    private List<DistractionNode> _distractions;

    public override void _Ready()
    {
        _distractions = new List<DistractionNode>();
    }

    public void Add(DistractionNode node)
    {
        _distractions.Add(node);
    }
    
    public DistractionNode RandomDistraction(RandomNumberGenerator rnd)
    {
        return _distractions[rnd.RandiRange(0, _distractions.Count-1)];
    }
}