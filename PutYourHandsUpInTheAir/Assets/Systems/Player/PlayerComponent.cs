using SystemBase;
using Systems.Tourist;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerComponent : GameComponent
    {
        public BoolReactiveProperty IsMoving;
        public ReactiveProperty<TouristBrainComponent> TargetedTourist = new ReactiveProperty<TouristBrainComponent>();
        public ReactiveProperty<TouristBrainComponent> LastTargetedTourist = new ReactiveProperty<TouristBrainComponent>();
        public Vector3 TargetVector;

        public bool TouristOnlyMode;
        public bool TouristClicked { get; set; }
    }
}