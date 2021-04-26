using SystemBase;
using Systems.Player;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;

namespace Systems.Distractions.ForEnd
{
    [GameSystem]
    public class MoneyDistractionSystem : GameSystem<MoneyDistractionTouristComponent, PlayerComponent>
    {
        public override void Register(MoneyDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(touristBrain.transform.position));
            touristBrain.States.GoToState(new Interacting());
            component.CreatedFrom.HasFired = component.CreatedFrom.FireOnce;

            component.LastDistractionProgressTime = component.CreatedFrom.DistractionInteractionDuration;

            WaitOn<PlayerComponent>()
                .Subscribe(player => StartPlayerCollisionTracking(component, player))
                .AddToLifecycleOf(component);
        }

        public override void Register(PlayerComponent component)
        {
            RegisterWaitable(component);
        }

        private void StartPlayerCollisionTracking(
            MoneyDistractionTouristComponent component,
            PlayerComponent player)
        {
            player.OnTriggerEnterAsObservable()
                .Subscribe(CollideWithPlayer)
                .AddToLifecycleOf(component);
        }

        private void CollideWithPlayer(Collider coll)
        {
            Object.Destroy(coll.gameObject.GetComponent<MoneyDistractionTouristComponent>());
            var tourist = coll.gameObject.GetComponent<TouristBrainComponent>();
            if (!tourist) return;
            
            tourist.States.GoToState(new GoingBackToIdle(tourist.transform.position));
            tourist.States.GoToState(new Idle(tourist.transform.position));
            tourist.HasPaid = true;
        }
    }
}
