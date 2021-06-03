using SystemBase.StateMachineBase;
using Systems.Movement;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Idle))]
    public class GoingIntoLevel : BaseState<TouristBrainComponent>
    {
        private readonly TouristBrainComponent _tourist;
        private readonly Vector2 _center;

        public GoingIntoLevel(TouristBrainComponent tourist)
        {
            _tourist = tourist;
            _center = Random.insideUnitCircle;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var movement = _tourist.GetComponent<TwoDeeMovementComponent>();
            _tourist.UpdateAsObservable()
                .Subscribe(_ => MoveTouristToCenter(movement, context))
                .AddTo(this);
        }

        private void MoveTouristToCenter(TwoDeeMovementComponent movement, StateContext<TouristBrainComponent> context)
        {
            var currentPosition = (Vector2) movement.transform.position;
            var distance = _center - currentPosition;
            if (distance.sqrMagnitude < 1.3f)
            {
                movement.SlowStop();
                context.GoToState(new Idle(_center, _tourist));
            }
            else
            {
                movement.Direction.Value = distance.normalized;
            }
        }
    }
}