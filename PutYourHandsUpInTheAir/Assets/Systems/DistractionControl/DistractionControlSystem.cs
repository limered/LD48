using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.Distractions;
using Systems.Distractions2;
using Systems.Room;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Systems.DistractionControl
{
    [GameSystem]
    public class DistractionControlSystem : GameSystem<DistractionControlConfig, DistractionComponent, RoomComponent>
    {
        private readonly RandomTouristFinder _randomTouristFinder = new RandomTouristFinder();
        private readonly ReactiveProperty<RoomComponent> _currentRoom = new ReactiveProperty<RoomComponent>();

        public override void Register(DistractionControlConfig component)
        {
            RegisterWaitable(component);

            Observable
                .Interval(TimeSpan.FromSeconds(component.DistractionTimerValue))
                .Where(_ => _currentRoom.Value.State.CurrentState.Value is RoomRunning)
                .Subscribe(_ => TriggerDistractions(component))
                .AddTo(component);
        }

        public override void Register(DistractionComponent component)
        {
            WaitOn<DistractionControlConfig>()
                .Subscribe(config => RegisterComponentToTrigger(component, config))
                .AddTo(component);
        }

        public override void Register(RoomComponent component)
        {
            _currentRoom.Value = component;
        }

        private void TriggerDistractions(DistractionControlConfig component)
        {
            Queue<TouristBrainComponent> tourists = _randomTouristFinder.FindTouristsWithoutDistraction();
            List<DistractionComponent> distractionComponents = component
                .DistractionComponents
                .Where(c => !c.HasFired)
                .ToList();
            var distractionGiven = 0;
            while (tourists.Any() 
                   && distractionComponents.Any() 
                   && distractionGiven < component.TouristsPerDistraction)
            {
                var distractionComponent = distractionComponents[(int) (Random.value * distractionComponents.Count())];
                AddDistractionToRandomStranger(distractionComponent, tourists.Dequeue());
                distractionGiven++;
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
                case DistractionType.Butterfly:
                    var butterflyComp = brain.AddComponent<ButterflyDistractionTouristComponent>();
                    butterflyComp.CreatedFrom = component;
                    butterflyComp.LastDistractionProgressTime = component.DistractionInteractionDuration;
                    return;
                case DistractionType.Spider:
                    var spiderComp = brain.AddComponent<SpiderDistractionTouristComponent>();
                    spiderComp.CreatedFrom = component;
                    spiderComp.LastDistractionProgressTime = component.DistractionInteractionDuration;
                    return;
                case DistractionType.Money:
                    var moneyComp = brain.AddComponent<MoneyDistractionTouristComponent>();
                    moneyComp.CreatedFrom = component;
                    moneyComp.LastDistractionProgressTime = component.DistractionInteractionDuration;
                    return;
                case DistractionType.Bus:
                    var busComp = brain.AddComponent<BusDistractionTouristComponent>();
                    busComp.CreatedFrom = component;
                    busComp.LastDistractionProgressTime = component.DistractionInteractionDuration;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
