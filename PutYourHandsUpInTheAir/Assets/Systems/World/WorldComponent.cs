using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.World
{
    public class WorldComponent : GameComponent
    {
        public GameObject FirstLevel;
        public GameObject LastLevel;
        public GameObject[] EasyRooms;
        public GameObject[] MediumRooms;
        public GameObject[] HardRooms;

        public Color FinalJungleColor;

        public int MaxLevelCount = 6;
        public IntReactiveProperty CurrentLevelNr = new IntReactiveProperty(0);
        public ReactiveProperty<GameObject> CurrentLevel = new ReactiveProperty<GameObject>();

        public bool PlayTestMode = false;
        public GameObject PlayTestRoomPrefab;
    }
}
