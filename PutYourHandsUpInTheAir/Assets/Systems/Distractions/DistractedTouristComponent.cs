using SystemBase;
using Systems.DistractionControl;
using Systems.Distractions2;
using UniRx;
using UnityEngine;

namespace Systems.Distractions
{
    public class DistractedTouristComponent : GameComponent
    {
        public FloatReactiveProperty DistractionProgress = new FloatReactiveProperty(0);
        public DistractionComponent CreatedFrom;
        public DistractionType CurrentDistractionType => CreatedFrom.DistractionType;
        public Color ProgressColor => CreatedFrom.DistractionProgressColor;
        public float MaxProgressTime => CreatedFrom.DistractionInteractionDuration;
        public Transform InteractionPosition => CreatedFrom.InteractionPosition;
    }
}
