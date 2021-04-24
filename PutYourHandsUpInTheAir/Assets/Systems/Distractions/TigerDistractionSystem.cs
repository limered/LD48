using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.DistractionControl;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Utils.DotNet;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class TigerDistractionSystem : GameSystem<TigerDistractionComponent, TigerDistractionTouristComponent>
    {
        private RandomTouristFinder _randomTouristFinder = new RandomTouristFinder();

        public override void Register(TigerDistractionComponent component)
        {
            component.StartDistraction
                .Subscribe(_ => AddDistractionToRamdomStranger(component))
                .AddTo(component);
        }

        private void AddDistractionToRamdomStranger(TigerDistractionComponent component)
        {
            var touristBrain = _randomTouristFinder.FindRandomTouristWithoutDistraction();

            if (!touristBrain) return;

            touristBrain.States.GoToState(new PickingInterest());
            touristBrain.AddComponent<TigerDistractionTouristComponent>();
        }

        public override void Register(TigerDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction());
            touristBrain.States.CurrentState
                .Where(state => state is Interacting)
                .Subscribe(_ => StartInteracting(component))
                .AddTo(component);
        }

        private void StartInteracting(TigerDistractionTouristComponent component)
        {
            throw new System.NotImplementedException();
        }
    }
}
