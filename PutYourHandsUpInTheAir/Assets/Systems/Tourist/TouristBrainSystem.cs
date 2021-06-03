using SystemBase;
using Systems.Room.Events;
using Systems.Tourist.States;
using UniRx;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristBrainSystem : GameSystem<TouristBrainComponent>
    {
        public override void Register(TouristBrainComponent component)
        {
            MessageBroker.Default.Receive<RoomTimerEndedAction>()
                .Subscribe(msg => component.StateContext.GoToState(new WalkingOutOfLevel(msg.WalkOutPosition)))
                .AddTo(component);
        }
    }
}