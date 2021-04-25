using SystemBase;
using Systems.Movement;
using Systems.Player;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Systems.Distractions
{
    [GameSystem]
    public class SpiderDistractionSystem : GameSystem<SpiderDistractionTouristComponent, PlayerComponent>
    {
        public override void Register(SpiderDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition.position));
            touristBrain.States.CurrentState
                .Where(state => state is Interacting)
                .Subscribe(_ => StartInteracting(component))
                .AddTo(component);
        }

        public override void Register(PlayerComponent component)
        {
            RegisterWaitable(component);
        }

        private void StartInteracting(SpiderDistractionTouristComponent component)
        {
            const float poisoningTime = 1;
            component.PoisoningProgressTime = poisoningTime;
            component.UpdateAsObservable()
                .Where(_ => component && !component.IsPoisoned)
                .Subscribe(_ => UpdatePoisoningTimer(component, poisoningTime))
                .AddTo(component);

            component.UpdateAsObservable()
                .Where(_ => component && component.IsPoisoned)
                .Subscribe(_ => UpdatePoisonedTimer(component))
                .AddTo(component);

            SystemUpdate().Where(_ => component).Subscribe(_ => DoStuff(component)).AddTo(component);

            WaitOn<PlayerComponent>()
                .Subscribe(player => StartPlayerCollisionTracking(component, player))
                .AddToLifecycleOf(component);
        }

        private static void DoStuff(SpiderDistractionTouristComponent component)
        {
            var movement = component.GetComponent<MovementComponent>();
            if (!component.IsPoisoned)
            {
                component.RandomPoisonedPosition = movement.transform.position;
                return;
            }

            var delta = component.RandomPoisonedPosition - (Vector2) movement.transform.position;
            if (delta.sqrMagnitude < 0.01)
            {
                component.RandomPoisonedPosition = (Vector2) movement.transform.position + Random.insideUnitCircle;
                delta = component.RandomPoisonedPosition - (Vector2) movement.transform.position;
            }

            var tourist = component.GetComponent<TouristBrainComponent>();
            movement.Speed = Random.Range(tourist.idleSpeed, tourist.idleSpeed * 2);
            movement.Direction.Value = delta;
        }

        private static void UpdatePoisoningTimer(SpiderDistractionTouristComponent comp, float poisoningTime)
        {
            comp.PoisoningProgressTime -= Time.deltaTime;

            if (comp.PoisoningProgressTime <= 0)
            {
                comp.DistractionProgress.Value = 0;
                comp.IsPoisoned = true;
                return;
            }

            comp.DistractionProgress.Value = 1 - comp.PoisoningProgressTime / poisoningTime;
        }

        private static void UpdatePoisonedTimer(SpiderDistractionTouristComponent comp)
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

        private void StartPlayerCollisionTracking(
            SpiderDistractionTouristComponent component,
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
    }
}