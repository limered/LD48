using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Distractions;
using Systems.Distractions.States;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Interacting), typeof(GoingBackToIdle), typeof(WalkingOutOfLevel))]
    public class GoingToDistraction : BaseState<TouristBrainComponent>
    {
        public GoingToDistraction(DistractionOriginComponent distraction)
        {
            Distraction = distraction;
        }

        public DistractionOriginComponent Distraction { get; }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var distractable = context.Owner.GetComponent<DistractableComponent>();
            distractable.Origin = Distraction;
            distractable.DistractionType.Value = Distraction.DistractionType;

            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();
            context.Owner.UpdateAsObservable()
                .Subscribe(_ => GoToDistraction(context, movement))
                .AddTo(this);

            Distraction.StateContext.CurrentState.Where(state => state is DistractionStateAborted)
                .Subscribe(_ => context.GoToState(new GoingBackToIdle(Random.insideUnitCircle)))
                .AddTo(this);
        }

        private void GoToDistraction(StateContext<TouristBrainComponent> context, TwoDeeMovementComponent movement)
        {
            var touristPosition = context.Owner.transform.position;
            var distractionPosition = Distraction.TouristInteractionCollider.ClosestPointOnBounds(touristPosition);
            
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