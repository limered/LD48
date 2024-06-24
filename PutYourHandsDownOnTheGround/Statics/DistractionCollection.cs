using System.Collections.Generic;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public static class DistractionCollection 
{
    private static List<DistractionNode> _distractions = new();

    public static void Add(DistractionNode node)
    {
        _distractions.Add(node);
    }
    
    public static DistractionNode RandomDistraction(RandomNumberGenerator rnd)
    {
        return _distractions[rnd.RandiRange(0, _distractions.Count-1)];
    }

    public static void Reset()
    {
        _distractions = new List<DistractionNode>();
    }
}