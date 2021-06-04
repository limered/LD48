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
    public class DistractionControlSystem : GameSystem<DistractionControlConfig, DistractionOriginComponent, RoomComponent>
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

        public override void Register(DistractionOriginComponent originComponent)
        {
            WaitOn<DistractionControlConfig>()
                .Subscribe(config => RegisterComponentToTrigger(originComponent, config))
                .AddTo(originComponent);
        }

        public override void Register(RoomComponent component)
        {
            _currentRoom.Value = component;
        }

        private void TriggerDistractions(DistractionControlConfig component)
        {
            Queue<TouristBrainComponent> tourists = _randomTouristFinder.FindTouristsWithoutDistraction();
            List<DistractionOriginComponent> distractionComponents = component
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

        private void RegisterComponentToTrigger(DistractionOriginComponent originComponent, DistractionControlConfig config)
        {
            config.DistractionComponents.Add(originComponent);
        }

        private void AddDistractionToRandomStranger(DistractionOriginComponent originComponent, TouristBrainComponent brain)
        {
            if (!brain || !originComponent) return;

            switch (originComponent.DistractionType)
            {
                case DistractionType.Money:
                    var moneyComp = brain.AddComponent<MoneyDistractionTouristComponent>();
                    moneyComp.CreatedFrom = originComponent;
                    moneyComp.LastDistractionProgressTime = originComponent.DistractionInteractionDuration;
                    return;
                case DistractionType.Bus:
                    var busComp = brain.AddComponent<BusDistractionTouristComponent>();
                    busComp.CreatedFrom = originComponent;
                    busComp.LastDistractionProgressTime = originComponent.DistractionInteractionDuration;
                    return;
            }
        }
    }
}
