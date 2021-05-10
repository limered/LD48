using SystemBase;
using Systems.Movement;
using Systems.Tourist;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    [RequireComponent(typeof(TwoDeeMovementComponent))]
    public class PlayerComponent : GameComponent
    {
        public ReactiveProperty<TouristBrainComponent> TargetedTourist = new ReactiveProperty<TouristBrainComponent>();
        public ReactiveProperty<TouristBrainComponent> LastTargetedTourist = new ReactiveProperty<TouristBrainComponent>();
        public Vector3 TargetVector;

        public bool TouristOnlyMode;
        public bool TouristClicked { get; set; }
    }
}