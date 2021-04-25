using System;
using SystemBase;
using Systems.Distractions;
using Systems.Tourist.States;
using UniRx;
using Unity.VisualScripting;

namespace Systems.DistractionControl
{
    [GameSystem]
    public class DistractionControlSystem : GameSystem<DistractionControlConfig, DistractionComponent>
    {
        private readonly RandomTouristFinder _randomTouristFinder = new RandomTouristFinder();

        public override void Register(DistractionControlConfig component)
        {
            RegisterWaitable(component);

            Observable
                .Interval(TimeSpan.FromSeconds(component.DistractionTimerValue))
                .Subscribe(_ => component.DistractionTrigger.Execute())
                .AddTo(component);
        }

        public override void Register(DistractionComponent component)
        {
            WaitOn<DistractionControlConfig>()
                .Subscribe(config => RegisterComponentToTrigger(component, config))
                .AddTo(component);

            component.StartDistraction
                .Subscribe(_ => AddDistractionToRandomStranger(component))
                .AddTo(component);
        }

        private void RegisterComponentToTrigger(DistractionComponent component, DistractionControlConfig config)
        {
            config.DistractionTrigger
                .Subscribe(_ => component.StartDistraction.Execute())
                .AddTo(component);
        }

        private void AddDistractionToRandomStranger(DistractionComponent component)
        {
            var touristBrain = _randomTouristFinder.FindRandomTouristWithoutDistraction();

            if (!touristBrain) return;

            touristBrain.States.GoToState(new PickingInterest());

            switch (component.DistractionType)
            {
                case DistractionType.Tiger:
                    var comp = touristBrain.AddComponent<TigerDistractionTouristComponent>();
                    comp.CurrentDistractionType = component.DistractionType;
                    comp.LastDistractionProgressTime = component.DistractionInteractionDuration;
                    comp.MaxProgressTime = component.DistractionInteractionDuration;
                    comp.ProgressColor = component.DistractionProgressColor;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum DistractionType
    {
        None,
        Tiger,
    }
}
