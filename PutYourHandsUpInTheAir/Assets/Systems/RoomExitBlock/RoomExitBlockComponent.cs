using SystemBase;
using UnityEngine;

namespace Systems.RoomExitBlock
{
    public class RoomExitBlockComponent : GameComponent
    {
        public Sprite[] sprites;
        public SpriteRenderer spriteRenderer;
        public Transform leftBorder;
        public Transform rightBorder;
    }
}