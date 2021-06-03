using SystemBase.StateMachineBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Idle), typeof(WalkingOutOfLevel))]
    public class GoingBackToIdle : BaseState<TouristBrainComponent>
    {
        public Vector2 GatherPosition { get; }

        public GoingBackToIdle(Vector2 gatherPosition)
        {
            GatherPosition = gatherPosition;
        }
        
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();
            context.Owner.UpdateAsObservable()
                .Subscribe(_ => MoveTouristToCenter(movement, context))
                .AddTo(this);
        }

        private void MoveTouristToCenter(TwoDeeMovementComponent movement, StateContext<TouristBrainComponent> context)
        {
            var currentPosition = (Vector2)movement.transform.position;
            var distance = GatherPosition - currentPosition;
            if (distance.sqrMagnitude < 1.3f)
            {
                movement.SlowStop();
                context.GoToState(new Idle(GatherPosition));
            }
            else
            {
                movement.Direction.Value = distance.normalized;
            }
        }

        public override string ToString()
        {
            return $"GoingBackToIdle({GatherPosition})";
        }
    }
}