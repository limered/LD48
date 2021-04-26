﻿using SystemBase;
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
                .Where(_ => component.GetComponent<TouristBrainComponent>().States.CurrentState.Value is Interacting)
                .Subscribe(_ => UpdateTimer(component))
                .AddToLifecycleOf(component);
        }

        private void StartPlayerCollisionTracking(
            TigerDistractionTouristComponent component, 
            PlayerComponent player)
        {
            player.OnTriggerEnterAsObservable()
                .Subscribe(coll => CollideWithPlayer(coll, player))
                .AddToLifecycleOf(component);

            player.OnTriggerStayAsObservable()
                .Subscribe(coll => DuringTouristCollision(coll, player))
                .AddToLifecycleOf(component);

            player.OnTriggerExitAsObservable()
                .Subscribe(coll => StopCollideWithPlayer(coll, player))
                .AddToLifecycleOf(component);
        }

        private void DuringTouristCollision(Collider coll, PlayerComponent player)
        {
            var tourist = coll.gameObject.GetComponent<TouristBrainComponent>();
            if (tourist && player.TargetedTourist == tourist)
            {
                var distractionComponent = coll.gameObject.GetComponent<TigerDistractionTouristComponent>();
                if (distractionComponent  && tourist.States.CurrentState.Value is Idle)
                {
                    Object.Destroy(distractionComponent);
                }
            }
        }

        private void StopCollideWithPlayer(Collider coll, PlayerComponent player)
        {
            var tourist = coll.gameObject.GetComponent<TouristBrainComponent>();
            if (tourist && player.LastTargetetTourist == tourist)
            {
                var distractionComponent = coll.gameObject.GetComponent<TigerDistractionTouristComponent>();
                if(distractionComponent)
                {
                    distractionComponent.LastDistractionProgressTime =
                        distractionComponent.CreatedFrom.DistractionInteractionDuration;

                    tourist.States
                        .GoToState(new GoingToAttraction(distractionComponent.InteractionPosition.position));
                }
            }
        }

        private void CollideWithPlayer(Collider coll, PlayerComponent player)
        {
            var tourist = coll.gameObject.GetComponent<TouristBrainComponent>();
            if (tourist && player.TargetedTourist == tourist)
            {
                var distractionComponent = coll.gameObject.GetComponent<TigerDistractionTouristComponent>();
                distractionComponent.LastDistractionProgressTime =
                    distractionComponent.CreatedFrom.DistractionInteractionDuration;

                tourist.States
                    .GoToState(new GoingBackToIdle(Random.insideUnitCircle));
            }
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
