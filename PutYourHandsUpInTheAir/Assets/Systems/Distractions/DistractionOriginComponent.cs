using SystemBase;
using SystemBase.StateMachineBase;
using UniRx;
using UnityEngine;

namespace Systems.Distractions
{
    public class DistractionOriginComponent : GameComponent
    {
        public DistractionType DistractionType;
        public float DistractionInteractionDuration;
        public Color DistractionProgressColor;
        public Transform InteractionPosition;
        public SphereCollider InteractionCollider;
        public bool FireOnce = false;

        public bool HasFired { get; set; }

        public ReactiveCommand DistractionStopped = new ReactiveCommand();
        public StateContext<DistractionOriginComponent> StateContext;

        private void Awake()
        {
            StateContext = new StateContext<DistractionOriginComponent>(this);
        }
    }
}