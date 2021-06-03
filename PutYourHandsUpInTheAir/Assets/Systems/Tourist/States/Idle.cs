using SystemBase.StateMachineBase;
using Systems.Movement;
using Systems.Room.Events;
using Assets.Utils.Math;
using UniRx;
using UniRx.Triggers;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(PickingInterest), typeof(WalkingOutOfLevel),
        /*just for testing*/typeof(Dead) /*just for testing*/)]
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

        public override string ToString()
        {
            return $"{nameof(Idle)}({_idlePosition})";
        }
    }
}