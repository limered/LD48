using SystemBase;
using UnityEngine;

namespace Systems.Tourist
{
    public class TouristBodyComponent : GameComponent
    {
        public GameObject livingBody;
        public SpriteRenderer head;
        public SpriteRenderer body;
        
        public GameObject blood;
    }
}