using SystemBase;
using SystemBase.StateMachineBase;
using UniRx;
using UnityEngine;

namespace Systems.Room
{
    public class RoomComponent : GameComponent
    {
        public Vector2 SpawnInPosition;
        public Vector2 GatherPosition;
        public Vector2 SpawnOutPosition;
        public float MaxTimeInRoom = 30;
        public float TimeLeftInRoom;
        public FloatReactiveProperty RoomTimeProgress = new FloatReactiveProperty();
        public StateContext<RoomComponent> State = new StateContext<RoomComponent>();
    }
}
