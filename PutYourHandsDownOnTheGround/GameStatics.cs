using Godot;

namespace IsThisATiger2.Empty;

public static class GameStatics
{
    public static readonly Vector2 InterestCooldown = new(5, 10);
    public const int DistractionDistance = 50;
    public const float InterestPickingProbability = 0.01f;

    public const float HeroSpeed = 20000;
    public const float TouristIdleSpeed = 5000;
    public const float TouristDistractionSpeed = 10000;
}