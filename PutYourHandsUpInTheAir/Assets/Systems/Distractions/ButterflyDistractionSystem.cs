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
    public class ButterflyDistractionSystem : GameSystem<ButterflyDistractionTouristComponent, PlayerComponent>
    {
        public override void Register(ButterflyDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition.position));
            touristBrain.States.CurrentState
                .Where(state => state is Interacting)
                .Subscribe(_ => StartInteracting(component))
                .AddToLifecycleOf(component);

            WaitOn<PlayerComponent>()
                .Subscribe(player => StartPlayerCollisionTracking(component, player))
                .AddToLifecycleOf(component);
        }

        private void StartPlayerCollisionTracking(ButterflyDistractionTouristComponent component, PlayerComponent player)
        {
            player.OnTriggerEnterAsObservable()
                .Subscribe(coll => CollideWithPlayer(coll, player))
                .AddToLifecycleOf(component);
        }

        private void CollideWithPlayer(Collider coll, PlayerComponent player)
        {
            var tourist = coll.gameObject.GetComponent<TouristBrainComponent>();
            if (!tourist || tourist != player.TargetedDistraction.Value) return;

            var comp = coll.gameObject.GetComponent<ButterflyDistractionTouristComponent>();
            if (!comp) return;
            comp.DistractionProgress.Value = 1;
            tourist.States
                .GoToState(new GoingBackToIdle(Random.insideUnitCircle));
            Object.Destroy(comp);
        }

        public override void Register(PlayerComponent component)
        {
            RegisterWaitable(component);
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
