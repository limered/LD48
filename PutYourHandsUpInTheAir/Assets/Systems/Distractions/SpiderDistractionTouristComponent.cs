using UnityEngine;

namespace Systems.Distractions
{
    public class SpiderDistractionTouristComponent : DistractedTouristComponent
    {
        public float LastDistractionProgressTime { get; set; }
        public float PoisoningProgressTime { get; set; }
        public bool IsPoisoned { get; set; }
        public Vector2 RandomPoisonedPosition { get; set; }
    }
}