using SystemBase;
using Systems.Tourist;
using Systems.Tourist.States;
using GameState.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using Utils.Plugins;

namespace Systems.Room
{
    [GameSystem]
    public class RoomSystem : GameSystem<RoomComponent, TouristBrainComponent>
    {
        public override void Register(RoomComponent component)
        {
            RegisterWaitable(component);
            component.State.Start(new RoomCreate());

            SystemUpdate(component)
                .Where(compo => compo.State.CurrentState.Value is RoomRunning)
                .Subscribe(ProgressRoom)
                .AddToLifecycleOf(component);

            component.TimeLeftInRoom = component.MaxTimeInRoom;
        }

        private void ProgressRoom(RoomComponent room)
        {
            room.TimeLeftInRoom -= Time.deltaTime;
            room.RoomTimeProgress.Value = 1 - room.TimeLeftInRoom / room.MaxTimeInRoom;

            if (room.TimeLeftInRoom <= 0)
            {
                room.State.GoToState(new RoomWalkOut());
                // Wait for allTourists walked out
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
