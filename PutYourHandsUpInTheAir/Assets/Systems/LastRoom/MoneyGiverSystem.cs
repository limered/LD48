using SystemBase;
using UniRx;
using UniRx.Triggers;

namespace Systems.LastRoom
{
    [GameSystem]
    public class MoneyGiverSystem : GameSystem<MoneyGiverComponent>
    {
        public override void Register(MoneyGiverComponent component)
        {
            MessageBroker.Default.Receive<AllTouristEnteredLastRoomMessage>()
                .Select(_ => component)
                .Subscribe(StartPaymentWish)
                .AddTo(component);
        }

        private void StartPaymentWish(MoneyGiverComponent moneyGiver)
        {
            moneyGiver.WantsToPay = true;
            moneyGiver.OnTriggerEnterAsObservable()
                .Where(collider => !moneyGiver.HasPaid && collider.gameObject.CompareTag("Player"))
                .Select(_ => moneyGiver)
                .Subscribe(Pay)
                .AddTo(moneyGiver);
        }

        private void Pay(MoneyGiverComponent moneyGiver)
        {
            moneyGiver.WantsToPay = false;
            moneyGiver.HasPaid = true;
        }
    }
}
