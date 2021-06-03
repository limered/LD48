using SystemBase.StateMachineBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(WalkedOut))]
    public class WalkingOutOfLevel : BaseState<TouristBrainComponent>
    {
        public Transform Target { get; }

        public WalkingOutOfLevel(Transform transform)
        {
            Target = transform;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();
            context.Owner.UpdateAsObservable()
                .Subscribe(_ => GoIntoTheLight(context, movement))
                .AddTo(this);
        }

        private void GoIntoTheLight(StateContext<TouristBrainComponent> context, TwoDeeMovementComponent movement)
        {
            var targetPosition = Target.position;
            var touristPosition = context.Owner.transform.position;
            var directionVec = targetPosition - touristPosition;
            if (directionVec.sqrMagnitude < 0.1f)
            {
                context.GoToState(new WalkedOut());
            }
            else
            {
                movement.Direction.Value = directionVec.normalized;
            }
        }
    }
}