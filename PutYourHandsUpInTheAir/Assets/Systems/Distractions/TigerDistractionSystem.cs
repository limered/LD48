using SystemBase;
using Systems.DistractionControl;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class TigerDistractionSystem : GameSystem<TigerDistractionComponent, TigerDistractionTouristComponent>
    {
        private readonly RandomTouristFinder _randomTouristFinder = new RandomTouristFinder();

        public override void Register(TigerDistractionComponent component)
        {
            component.StartDistraction
                .Subscribe(_ => AddDistractionToRandomStranger())
                .AddTo(component);
        }

        private void AddDistractionToRandomStranger()
        {
            var touristBrain = _randomTouristFinder.FindRandomTouristWithoutDistraction();

            if (!touristBrain) return;

            touristBrain.States.GoToState(new PickingInterest());
            touristBrain.AddComponent<TigerDistractionTouristComponent>();
        }

        public override void Register(TigerDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(new Vector2(5, 5)));
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
