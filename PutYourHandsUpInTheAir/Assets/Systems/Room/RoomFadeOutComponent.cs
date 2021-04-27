using SystemBase;
using UnityEngine;

namespace Systems.Room
{
    public class RoomFadeOutComponent : GameComponent
    {
        public RoomComponent Room;
        public float FadeTime = 2;
        public Color FadeToColor { get; set; }
    }
}
