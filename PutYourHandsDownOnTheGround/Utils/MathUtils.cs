using Godot;

namespace IsThisATiger2.Empty.Utils;

public static class MathUtils
{
    public static Vector2 RandomPointOnUnitRadius(this RandomNumberGenerator rnd)
    {
        var rot = rnd.RandfRange(0, 360);
        var x = Mathf.Cos(rot);
        var y = Mathf.Sin(rot);
        return new Vector2(x, y);
    }
}