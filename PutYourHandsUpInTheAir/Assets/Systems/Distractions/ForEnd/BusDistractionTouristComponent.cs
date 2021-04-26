using Systems.Distractions;
using UnityEngine;

public class BusDistractionTouristComponent : DistractedTouristComponent
{
    public float LastDistractionProgressTime { get; set; }
    public Vector2 BusStopPosition { get; set; }
}
