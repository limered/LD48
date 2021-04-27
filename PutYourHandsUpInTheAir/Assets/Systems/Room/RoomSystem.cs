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
        private float _animationTime = 5;

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

            MessageBroker.Default
                .Receive<RoomEverybodyDied>()
                .Subscribe(_ => ShowRestartMessage(room))
                .AddToLifecycleOf(room);
        }

        private void ShowRestartMessage(RoomComponent room)
        {
            Observable.Timer(TimeSpan.FromSeconds(_animationTime))
                .Subscribe(_ =>
                {
                    room.AllDiesBubble.SetActive(true);
                })
                .AddToLifecycleOf(room);
        }

        private void ProgressRoom(RoomComponent room)
        {
            room.TimeLeftInRoom -= Time.deltaTime;
            room.RoomTimeProgress.Value = 1 - room.TimeLeftInRoom / room.MaxTimeInRoom;

            if (!(room.TimeLeftInRoom <= 0)) return;

            room.State.GoToState(new RoomWalkOut());
            MessageBroker.Default
                .Receive<RoomAllTouristsLeft>()
                .Subscribe(_ => room.State.GoToState(new RoomDestroy()))
                .AddToLifecycleOf(room);
        }
    }
}
