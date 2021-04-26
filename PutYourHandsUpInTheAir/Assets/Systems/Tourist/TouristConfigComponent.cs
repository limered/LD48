using SystemBase;
using UnityEngine;

namespace Systems.Tourist
{
    public class TouristConfigComponent : GameComponent
    {
        public int initialTouristCount = 10;
        public GameObject touristPrefab;
        public Sprite[] topParts = new Sprite[0];
        public Sprite[] bottomParts = new Sprite[0];
    }
}