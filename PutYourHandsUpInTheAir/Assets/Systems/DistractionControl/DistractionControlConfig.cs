using SystemBase;
using UniRx;

namespace Systems.DistractionControl
{
    public class DistractionControlConfig : GameComponent
    {
        public float DistractionTimerValue = 5f;
        public ReactiveCommand DistractionTrigger = new ReactiveCommand();
    }
}