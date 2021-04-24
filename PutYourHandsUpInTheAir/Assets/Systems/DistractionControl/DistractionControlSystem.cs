using System;
using SystemBase;
using Systems.Distractions;
using UniRx;

namespace Systems.DistractionControl
{
    [GameSystem]
    public class DistractionControlSystem : GameSystem<DistractionControlConfig, BaseDistractionComponent>
    {
        public override void Register(DistractionControlConfig component)
        {
            Observable.Timer(TimeSpan.FromSeconds(component.DistractionTimerValue))
                .Subscribe(_ => component.DistractionTrigger.Execute())
                .AddTo(component);
        }

        public override void Register(BaseDistractionComponent component)
        {
            WaitOn<DistractionControlConfig>()
                .Subscribe(config => RegisterComponentToTrigger(component, config))
                .AddTo(component);
        }

        private void RegisterComponentToTrigger(BaseDistractionComponent component, DistractionControlConfig config)
        {
            config.DistractionTrigger
                .Subscribe(_ => component.StartDistraction.Execute())
                .AddTo(component);
        }
    }
}
