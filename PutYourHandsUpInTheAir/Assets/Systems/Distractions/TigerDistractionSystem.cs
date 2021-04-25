using SystemBase;
using Systems.DistractionControl;
using Systems.Player;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class TigerDistractionSystem : GameSystem<TigerDistractionTouristComponent, PlayerComponent>
    {
        public override void Register(TigerDistractionTouristComponent component)
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

        public override void Register(PlayerComponent component)
        {
            RegisterWaitable(component);
        }

        private void StartInteracting(TigerDistractionTouristComponent component)
        {
            component.UpdateAsObservable()
                .Where(_ => component)
                .Subscribe(_ => UpdateTimer(component))
                .AddToLifecycleOf(component);
        }

        private void StartPlayerCollisionTracking(
            TigerDistractionTouristComponent component, 
            PlayerComponent player)
        {
            player.OnTriggerEnterAsObservable()
                .Subscribe(CollideWithPlayer)
                .AddToLifecycleOf(component);
        }

        private void CollideWithPlayer(Collider coll)
        {
            Object.Destroy(coll.gameObject.GetComponent<DistractedTouristComponent>());
            coll.gameObject.GetComponent<TouristBrainComponent>()
                .States
                .GoToState(new GoingBackToIdle(Random.insideUnitCircle));
        }

        private void UpdateTimer(TigerDistractionTouristComponent comp)
        {
            comp.LastDistractionProgressTime -= Time.deltaTime;

            if (comp.LastDistractionProgressTime <= 0)
            {
                comp.DistractionProgress.Value = 1;
                comp.GetComponent<TouristBrainComponent>()
                    .States
                    .GoToState(new Dead());
                Object.Destroy(comp);
            }

            comp.DistractionProgress.Value = 1 - comp.LastDistractionProgressTime / comp.MaxProgressTime;
        }
    }
}
