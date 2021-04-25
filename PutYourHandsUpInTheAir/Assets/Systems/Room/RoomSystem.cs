using SystemBase;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using Utils.Plugins;

namespace Systems.Room
{
    [GameSystem]
    public class RoomSystem : GameSystem<RoomComponent, TouristBrainComponent>
    {
        public override void Register(RoomComponent component)
        {
            RegisterWaitable(component);
        }

        public override void Register(TouristBrainComponent component)
        {
            WaitOn<RoomComponent>()
                .Subscribe(room => ResetTourist(component, room))
                .AddToLifecycleOf(component);
        }

        private void ResetTourist(TouristBrainComponent component, RoomComponent room)
        {
            component.States.GoToState(new GoingIntoLevel());
            component.transform.position = room.SpawnInPosition;
        }
    }
}
