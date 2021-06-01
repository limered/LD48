using System;
using SystemBase;
using UniRx;

namespace Systems.Distractions2
{
    public class DistractableComponent : GameComponent
    {
        public ReactiveProperty<DistractionType> DistractionType = 
            new ReactiveProperty<DistractionType>(Distractions2.DistractionType.None);

        public IDistraction ActiveDistraction;
        public FloatReactiveProperty DistractionProgress = new FloatReactiveProperty(0);
        public IDisposable DistractionUpdateObservable { get; set; }
    }
}