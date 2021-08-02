using SystemBase;
using Systems.Room.Events;
using UniRx;

namespace Systems.LastRoom
{
    public class LastRoomSystem : GameSystem<LastRoomComponent>
    {
        private readonly ReactiveProperty<LastRoomComponent> _roomComponent = new ReactiveProperty<LastRoomComponent>();

        public override void Register(LastRoomComponent room)
        {
            _roomComponent.Value = room;

            MessageBroker.Default.Receive<RoomAllTouristsEntered>()
                .Subscribe()
                .AddTo(room);
        }
    }

    public class LastRoomComponent : GameComponent
    {
    }
}
