using SystemBase;
using Systems.Room.Events;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UnityEngine;
using Utils.Plugins;

namespace Systems.Room
{
    [GameSystem]
    public class RoomSystem : GameSystem<RoomComponent, TouristBrainComponent>
    {
        public override void Register(RoomComponent room)
        {
            RegisterWaitable(room);
            room.State.Start(new RoomCreate());

            SystemUpdate(room)
                .Where(compo => compo.State.CurrentState.Value is RoomRunning)
                .Subscribe(ProgressRoom)
                .AddToLifecycleOf(room);

            room.TimeLeftInRoom = room.MaxTimeInRoom;

            room.State.GoToState(new RoomWalkIn());
            MessageBroker.Default
                .Receive<RoomAllTouristsEntered>()
                .Subscribe(_ => room.State.GoToState(new RoomRunning()))
                .AddToLifecycleOf(room);
        }

        private void ProgressRoom(RoomComponent room)
        {
            room.TimeLeftInRoom -= Time.deltaTime;
            room.RoomTimeProgress.Value = 1 - room.TimeLeftInRoom / room.MaxTimeInRoom;

            if (room.TimeLeftInRoom <= 0)
            {
                room.State.GoToState(new RoomWalkOut());
                MessageBroker.Default
                    .Receive<RoomAllTouristsLeft>()
                    .Subscribe(_ => room.State.GoToState(new RoomDestroy()))
                    .AddToLifecycleOf(room);
            }
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
