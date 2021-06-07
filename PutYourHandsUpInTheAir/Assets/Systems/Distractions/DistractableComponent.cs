using System;
using SystemBase;
using UniRx;

namespace Systems.Distractions
{
    public class DistractableComponent : GameComponent
    {
        public ReactiveProperty<DistractionType> DistractionType =
            new ReactiveProperty<DistractionType>(Distractions.DistractionType.None);

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