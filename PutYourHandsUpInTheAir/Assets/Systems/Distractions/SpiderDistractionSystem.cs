using SystemBase;
using Systems.Distractions2;
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
            //touristBrain.StateContext.GoToState(new GoingToAttraction(component.InteractionPosition));
            touristBrain.StateContext.CurrentState
                .Where(state => state is Interacting)
                .Subscribe(_ => StartInteracting(component))
                .AddToLifecycleOf(component);
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
                .Where(_ => component.GetComponent<TouristBrainComponent>().StateContext.CurrentState.Value is Interacting)
                .Subscribe(_ => UpdatePoisoningTimer(component, poisoningTime))
                .AddToLifecycleOf(component);

            component.UpdateAsObservable()
                .Where(_ => component && component.IsPoisoned)
                .Where(_ => component.GetComponent<TouristBrainComponent>().StateContext.CurrentState.Value is Interacting)
                .Subscribe(_ => UpdatePoisonedTimer(component))
                .AddToLifecycleOf(component);

            SystemUpdate()
                .Where(_ => component)
                .Where(_ => component.GetComponent<TouristBrainComponent>().StateContext.CurrentState.Value is Interacting)
                .Subscribe(_ => DoStuff(component))
                .AddToLifecycleOf(component);
        }

        private static void DoStuff(SpiderDistractionTouristComponent component)
        {
            var movement = component.GetComponent<TwoDeeMovementComponent>();
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
                var touristComp = comp.GetComponent<TouristBrainComponent>();
                comp.DistractionProgress.Value = 1;
                touristComp
                    .StateContext
                    .GoToState(new Dead(DistractionType.Spider));
                Object.Destroy(comp);
            }

            comp.DistractionProgress.Value = 1 - comp.LastDistractionProgressTime / comp.MaxProgressTime;
        }
    }
}