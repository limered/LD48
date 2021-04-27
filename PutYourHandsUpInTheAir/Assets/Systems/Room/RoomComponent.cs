using SystemBase;
using SystemBase.StateMachineBase;
using UniRx;
using UnityEngine;

namespace Systems.Room
{
    public class RoomComponent : GameComponent
    {
        public GameObject SpawnInPosition;
        public GameObject GatherPosition;
        public GameObject SpawnOutPosition;
        public GameObject TouristGroup;
        public float MaxTimeInRoom = 30;
        public float TimeLeftInRoom;
        public GameObject AllDiesBubble;
        public FloatReactiveProperty RoomTimeProgress = new FloatReactiveProperty();
        public StateContext<RoomComponent> State = new StateContext<RoomComponent>();
        public StringReactiveProperty CurrentState = new StringReactiveProperty();
        
    }
}
