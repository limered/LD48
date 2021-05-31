using SystemBase;
using Systems.DistractionControl;
using Systems.Player;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class ButterflyDistractionSystem : GameSystem<ButterflyDistractionTouristComponent>
    {
        public override void Register(ButterflyDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition.position));
            touristBrain.States.CurrentState
                .Where(state => state is Interacting)
                .Subscribe(_ => StartInteracting(component))
                .AddToLifecycleOf(component);
        }

        private static void StartInteracting(ButterflyDistractionTouristComponent component)
        {
            component.UpdateAsObservable()
                .Where(_ => component)
                .Subscribe(_ => UpdateTimer(component))
                .AddToLifecycleOf(component);
        }

        private static void UpdateTimer(ButterflyDistractionTouristComponent comp)
        {
            comp.LastDistractionProgressTime -= Time.deltaTime;

            if (comp.LastDistractionProgressTime <= 0)
            {
                comp.DistractionProgress.Value = 1;
                comp.GetComponent<TouristBrainComponent>()
                    .States
                    .GoToState(new GoingBackToIdle(Vector2.zero));
                Object.Destroy(comp);
            }

            comp.DistractionProgress.Value = 1 - comp.LastDistractionProgressTime / comp.MaxProgressTime;
        }
    }
}
