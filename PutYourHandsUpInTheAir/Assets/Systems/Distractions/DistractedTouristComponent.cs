using SystemBase;
using Systems.DistractionControl;
using UniRx;
using UnityEngine;

namespace Systems.Distractions
{
    public class DistractedTouristComponent : GameComponent
    {
        public DistractionType CurrentDistractionType;
        public Color ProgressColor;
        public FloatReactiveProperty DistractionProgress = new FloatReactiveProperty(0);
        public float MaxProgressTime { get; set; }
        public Transform InteractionPosition { get; set; }
    }
}
