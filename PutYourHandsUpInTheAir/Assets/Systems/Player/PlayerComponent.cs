using SystemBase;
using Systems.DistractionControl;
using Systems.Movement;
using Systems.Tourist;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    [RequireComponent(typeof(TwoDeeMovementComponent))]
    public class PlayerComponent : GameComponent
    {
        public ReactiveProperty<DistractionComponent> TargetedDistraction = new ReactiveProperty<DistractionComponent>();
        public Vector3 TargetVector;
    }
}