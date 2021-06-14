using SystemBase;
using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Movement;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    [RequireComponent(typeof(TwoDeeMovementComponent))]
    public class PlayerComponent : GameComponent
    {
        public float DistractionCancelDistance = 1.0f;
        public ReactiveProperty<DistractionOriginComponent> TargetedDistraction = new ReactiveProperty<DistractionOriginComponent>();
        public Vector3 TargetVector;

        public StateContext<PlayerComponent> State;

        private void Awake()
        {
            State = new StateContext<PlayerComponent>(this);
        }
    }
}