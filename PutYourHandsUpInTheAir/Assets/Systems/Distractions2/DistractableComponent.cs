using System;
using SystemBase;
using Systems.DistractionControl;
using UniRx;
using Unity.VisualScripting;

namespace Systems.Distractions2
{
    public class DistractableComponent : GameComponent
    {
        public ReactiveProperty<DistractionType> DistractionType = 
            new ReactiveProperty<DistractionType>(Distractions2.DistractionType.None);

        public IDistraction ActiveDistractionStrategy;
        public DistractionOriginComponent Origin;
        public FloatReactiveProperty DistractionProgress = new FloatReactiveProperty(0);
        public IDisposable DistractionUpdateObservable { get; set; }

        private new void OnDestroy()
        {
            DistractionUpdateObservable?.Dispose();
        }
    }
}