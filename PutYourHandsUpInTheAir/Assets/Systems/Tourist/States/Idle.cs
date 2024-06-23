using Assets.Utils.Math;
using SystemBase.StateMachineBase;
using Systems.LastRoom;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(PickingInterest), typeof(WalkingOutOfLevel), typeof(Paying))]
    public class Idle : BaseState<TouristBrainComponent>
    {
        private readonly Vector2 _idlePosition;

        public Idle(Vector2 idlePosition)
        {
            _idlePosition = idlePosition;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();

            context.Owner.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    MoveAround(movement);
                    StartPickingUpInterest(context);
                })
                .AddTo(this);

            MessageBroker.Default.Receive<AllTouristEnteredLastRoomMessage>()
                .Subscribe(_ => context.GoToState(new Paying()))
                .AddTo(this);
        }

        public override string ToString()
        {
            return $"{nameof(Idle)}({_idlePosition})";
        }

        private void StartPickingUpInterest(StateContext<TouristBrainComponent> context)
        {
            var start = Random.value;
            if (start < context.Owner.IntrerestPropability)
            {
                context.GoToState(new PickingInterest());
            }
        }

        private void MoveAround(TwoDeeMovementComponent movement)
        {
            var stop = Random.value * 10;
            if (stop < 6)
            {
                movement.SlowStop();
            }
            else
            {
                var pos = _idlePosition + Random.insideUnitCircle;
                var rndMovement = (pos - (Vector2)movement.transform.position)
                    .Rotate(Random.Range(-90, 90))
                    .normalized;
                var delta = movement.Direction.Value + rndMovement;
                movement.Direction.Value = delta.normalized;
            }
        }
    }
}