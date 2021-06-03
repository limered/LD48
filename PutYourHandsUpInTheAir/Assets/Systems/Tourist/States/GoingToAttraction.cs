using SystemBase.StateMachineBase;
using Systems.DistractionControl;
using Systems.Distractions2;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Interacting), typeof(GoingBackToIdle), typeof(WalkingOutOfLevel))]
    public class GoingToAttraction : BaseState<TouristBrainComponent>
    {
        public DistractionOriginComponent Distraction { get; }

        public GoingToAttraction(DistractionOriginComponent distraction)
        {
            Distraction = distraction;
        }
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            context.Owner.GetComponent<DistractableComponent>().DistractionType.Value =
                Distraction.DistractionType;

            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();
            context.Owner.UpdateAsObservable()
                .Subscribe(_ => GoToDistraction(context, movement))
                .AddTo(this);
        }

        private void GoToDistraction(StateContext<TouristBrainComponent> context, TwoDeeMovementComponent movement)
        {
            var distractionPosition = Distraction.InteractionPosition.transform.position;
            var touristPosition = context.Owner.transform.position;
            var distanceVec = distractionPosition - touristPosition;
            if (distanceVec.sqrMagnitude < 0.1f)
            {
                context.GoToState(new Interacting(Distraction));
            }
            else
            {
                movement.Direction.Value = distanceVec.normalized;
            }
        }
    }
}