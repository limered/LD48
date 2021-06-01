using System.Collections.Generic;
using SystemBase;
using UniRx;

namespace Systems.DistractionControl
{
    public class DistractionControlConfig : GameComponent
    {
        public float DistractionTimerValue = 5f;
        public int TouristsPerDistraction = 2;
        public ReactiveCommand DistractionTrigger = new ReactiveCommand();
        public List<DistractionOriginComponent> DistractionComponents = new List<DistractionOriginComponent>();
    }
}