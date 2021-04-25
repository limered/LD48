using SystemBase;
using Systems.DistractionControl;
using Systems.Player;
using Systems.Tourist;
using Systems.Tourist.States;
using Assets.Utils.Math;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
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
                .AddTo(component);
        }

        private void StartInteracting(TigerDistractionTouristComponent component)
        {
            component.UpdateAsObservable()
                .Where(_ => component)
                .Subscribe(_ => UpdateTimer(component))
                .AddTo(component);

            WaitOn<PlayerComponent>()
                .Subscribe(player => StartPlayerCollisionTracking(component, player))
                .AddTo(component);
        }

        private void StartPlayerCollisionTracking(
            TigerDistractionTouristComponent component, 
            PlayerComponent player)
        {
            player.OnTriggerEnterAsObservable()
                .Subscribe(CollideWithPlayer)
                .AddTo(component);
        }

        private void CollideWithPlayer(Collider coll)
        {
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

        public override void Register(PlayerComponent component)
        {
            RegisterWaitable(component);
        }
    }
}
