using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.Distractions;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using Unity.VisualScripting;
using Random = UnityEngine.Random;


namespace Systems.DistractionControl
{
    public enum DistractionType
    {
        None,
        Tiger,
        Butterfly,
        Camera,
        Spider,
        Swamp,
        Money
    }

    [GameSystem]
    public class DistractionControlSystem : GameSystem<DistractionControlConfig, DistractionComponent>
    {
        private readonly RandomTouristFinder _randomTouristFinder = new RandomTouristFinder();

        public override void Register(DistractionControlConfig component)
        {
            RegisterWaitable(component);

            Observable
                .Interval(TimeSpan.FromSeconds(component.DistractionTimerValue))
                .Subscribe(_ => TriggerDistractions(component))
                .AddTo(component);
        }

        public override void Register(DistractionComponent component)
        {
            WaitOn<DistractionControlConfig>()
                .Subscribe(config => RegisterComponentToTrigger(component, config))
                .AddTo(component);
        }

        private void TriggerDistractions(DistractionControlConfig component)
        {
            Queue<TouristBrainComponent> tourists = _randomTouristFinder.FindTouristsWithoutDistraction();
            while (tourists.Any())
            {
                var distractionComponent = component
                    .DistractionComponents[(int) (Random.value * component.DistractionComponents.Count)];

                AddDistractionToRandomStranger(distractionComponent, tourists.Dequeue());
            }
        }

        private void RegisterComponentToTrigger(DistractionComponent component, DistractionControlConfig config)
        {
            config.DistractionComponents.Add(component);
        }

        private void AddDistractionToRandomStranger(DistractionComponent component, TouristBrainComponent brain)
        {
            if (!brain || !component) return;

            brain.States.GoToState(new PickingInterest());

            switch (component.DistractionType)
            {
                case DistractionType.Tiger:
                    var comp = brain.AddComponent<TigerDistractionTouristComponent>();
                    comp.CreatedFrom = component;
                    comp.LastDistractionProgressTime = component.DistractionInteractionDuration;
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
