using SystemBase.StateMachineBase;
using Systems.LastRoom;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(/*Waiting fo the bus*/)]
    public class Paying : BaseState<TouristBrainComponent>
    {

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            var moneyGiver = context.Owner.GetComponent<MoneyGiverComponent>();
            moneyGiver.WantsToPay = true;

            moneyGiver.OnTriggerEnterAsObservable()
                .Where(collider => !moneyGiver.HasPaid && collider.gameObject.CompareTag("Player"))
                .Select(_ => moneyGiver)
                .Subscribe(Pay)
                .AddTo(moneyGiver);
        }
        private void Pay(MoneyGiverComponent moneyGiver)
        {
            Debug.Log("payed");

            moneyGiver.WantsToPay = false;
            moneyGiver.HasPaid = true;
        }
    }
}
