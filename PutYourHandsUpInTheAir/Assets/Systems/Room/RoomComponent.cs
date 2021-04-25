using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.Room
{
    public class RoomComponent : GameComponent
    {
        public Vector2 SpawnInPosition;
        public Vector2 GatherPosition;
        public Vector2 SpawnOutPosition;
        public float TimeInRoom = 30;
        public float TimeLeftInRoom;
        public FloatReactiveProperty RoomTimeProgress = new FloatReactiveProperty();
    }
}
