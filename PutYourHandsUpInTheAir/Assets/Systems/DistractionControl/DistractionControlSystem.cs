using System;
using SystemBase;
using Systems.Distractions;
using Systems.Tourist.States;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Plugins;

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
                .Timer(TimeSpan.FromSeconds(component.DistractionTimerValue))
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
                    touristBrain.AddComponent<TigerDistractionTouristComponent>();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum DistractionType
    {
        Tiger,
    }
}
