using SystemBase;
using UnityEngine;

namespace Systems.Tourist
{
    public class TouristConfigComponent : GameComponent
    {
        public int initialTouristCount = 10;
        public GameObject touristPrefab;
        public Vector3 spawnPosition = new Vector3(0, -10, 0);
        public Sprite[] topParts = new Sprite[0];
        public Sprite[] bottomParts = new Sprite[0];
    }
}