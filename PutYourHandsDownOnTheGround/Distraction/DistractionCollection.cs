using System.Collections.Generic;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public static class DistractionCollection 
{
    private static readonly List<DistractionNode> Distractions = new();

    public static void Add(DistractionNode node)
    {
        Distractions.Add(node);
    }
    
    public static DistractionNode RandomDistraction(RandomNumberGenerator rnd)
    {
        return Distractions[rnd.RandiRange(0, Distractions.Count-1)];
    }
}