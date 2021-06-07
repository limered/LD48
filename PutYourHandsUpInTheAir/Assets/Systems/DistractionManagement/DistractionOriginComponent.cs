using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Distractions;
using UniRx;
using UnityEngine;

namespace Systems.DistractionManagement
{
    public class DistractionOriginComponent : GameComponent
    {
        public DistractionType DistractionType;
        public float DistractionInteractionDuration;
        public Color DistractionProgressColor;

        public ReactiveCommand DistractionAborted = new ReactiveCommand();
        public StateContext<DistractionOriginComponent> StateContext;

        public Collider TouristInteractionCollider;
        public Collider PlayerInteractionCollider;

        private void Awake()
        {
            StateContext = new StateContext<DistractionOriginComponent>(this);
        }
    }
}