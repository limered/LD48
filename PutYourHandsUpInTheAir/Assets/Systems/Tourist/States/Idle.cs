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
        private readonly TouristBrainComponent _tourist;

        public Idle(Vector2 idlePosition, TouristBrainComponent tourist)
        {
            _idlePosition = idlePosition;
            _tourist = tourist;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var movement = _tourist.GetComponent<TwoDeeMovementComponent>();
            _tourist.UpdateAsObservable()
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
            if (start < _tourist.IntrerestPropability)
            {
                context.GoToState(new PickingInterest(_tourist));
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