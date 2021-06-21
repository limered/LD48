using System;
using SystemBase;
using Systems.Room.Events;
using UniRx;
using UnityEngine;
using Utils.Plugins;

namespace Systems.Room
{
    [GameSystem]
    public class RoomSystem : GameSystem<RoomComponent>
    {
        public override void Register(RoomComponent room)
        {
            Debug.Log("New Room Added");

            RegisterWaitable(room);
            room.State.Start(new RoomCreate());
            room.State.CurrentState
                .Subscribe(state => room.CurrentState.Value = state.GetType().ToString())
                .AddToLifecycleOf(room);

            SystemUpdate(room)
                .Where(compo => compo.State.CurrentState.Value is RoomRunning)
                .Subscribe(ProgressRoom)
                .AddToLifecycleOf(room);

            room.TimeLeftInRoom = room.MaxTimeInRoom;

            MessageBroker.Default
                .Receive<RoomAllTouristsEntered>()
                .Subscribe(_ => room.State.GoToState(new RoomRunning()))
                .AddToLifecycleOf(room);

            room.State.GoToState(new RoomWalkIn());
        }

        private void ProgressRoom(RoomComponent room)
        {
            room.TimeLeftInRoom -= Time.deltaTime;
            room.RoomTimeProgress.Value = 1 - room.TimeLeftInRoom / room.MaxTimeInRoom;

            if (!(room.TimeLeftInRoom <= 0)) return;

            room.State.GoToState(new RoomWalkOut());
            MessageBroker.Default
                .Publish(new RoomTimerEndedAction{WalkOutPosition = room.SpawnOutPosition.transform });

            MessageBroker.Default
                .Receive<RoomAllTouristsLeft>()
                .Subscribe(_ => room.State.GoToState(new RoomDestroy()))
                .AddToLifecycleOf(room);
        }
    }
}
