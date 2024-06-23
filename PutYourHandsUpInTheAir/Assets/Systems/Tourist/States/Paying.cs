using System;
using SystemBase.StateMachineBase;
using Systems.LastRoom;
using Systems.Movement;
using Assets.Utils.Math;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(WaitingForTheBus))]
    public class Paying : BaseState<TouristBrainComponent>
    {

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();
            movement.SlowStop();
            context.Owner.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    MoveAround(movement, movement.gameObject.transform.position);
                })
                .AddTo(this);

            Observable.Timer(TimeSpan.FromSeconds(2f))
                .Subscribe(_ =>
                {
                    var moneyGiver = context.Owner.GetComponent<MoneyGiverComponent>();
                    moneyGiver.WantsToPay = true;
                    moneyGiver.OnTriggerEnterAsObservable()
                        .Where(collider => !moneyGiver.HasPaid && collider.gameObject.CompareTag("Player"))
                        .Select(_ => (moneyGiver, context))
                        .Subscribe(Pay)
                        .AddTo(moneyGiver);
                })
                .AddTo(this);
        }

        private void Pay((MoneyGiverComponent moneyGiver, StateContext<TouristBrainComponent> context) tuple)
        {
            (var moneyGiver, StateContext<TouristBrainComponent> context) = tuple;

            moneyGiver.WantsToPay = false;
            moneyGiver.HasPaid = true;

            context.GoToState(new WaitingForTheBus());
        }

        private void MoveAround(TwoDeeMovementComponent movement, Vector2 idlePosition)
        {
            var stop = Random.value * 10;
            if (stop < 9)
            {
                movement.SlowStop();
            }
            else
            {
                var pos = idlePosition + Random.insideUnitCircle;
                var rndMovement = (pos - (Vector2)movement.transform.position)
                    .Rotate(Random.Range(-90, 90))
                    .normalized;
                var delta = movement.Direction.Value + rndMovement;
                movement.Direction.Value = delta.normalized;
            }
        }
    }
}
