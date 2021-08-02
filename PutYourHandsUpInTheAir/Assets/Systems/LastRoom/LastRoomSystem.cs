using SystemBase;
using Systems.Room.Events;
using UniRx;

namespace Systems.LastRoom
{
    [GameSystem]
    public class LastRoomSystem : GameSystem<LastRoomComponent>
    {
        private readonly ReactiveProperty<LastRoomComponent> _roomComponent = new ReactiveProperty<LastRoomComponent>();

        public override void Register(LastRoomComponent room)
        {
            _roomComponent.Value = room;

            MessageBroker.Default.Receive<RoomAllTouristsEntered>()
                .Subscribe(SendAllTouristInLastRoomMessage)
                .AddTo(room);
        }

        private void SendAllTouristInLastRoomMessage(RoomAllTouristsEntered obj)
        {
            MessageBroker.Default.Publish(new AllTouristEnteredLastRoomMessage());
        }
    }
}
