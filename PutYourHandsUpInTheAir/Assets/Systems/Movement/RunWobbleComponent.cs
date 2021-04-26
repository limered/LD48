using SystemBase;
using SystemBase.StateMachineBase;

namespace Systems.Movement
{
    public class RunWobbleComponent : GameComponent
    {
        public float wobbleInterval = 0.1f;
        public float wobbleMaxExtend = 0.5f;
    }
}