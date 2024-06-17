using Godot;

namespace IsThisATiger2.Empty.Tourist;

public partial class TouristConfiguration : Resource
{
    [Export] public Texture2D Head;
    [Export] public Texture2D Body;
    [Export] public float IdleSpeed;
    [Export] public float MaxIdleSpeed;
}