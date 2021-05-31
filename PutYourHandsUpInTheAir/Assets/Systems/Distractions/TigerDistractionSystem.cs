using System;
using SystemBase;
using Systems.DistractionControl;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class TigerDistractionSystem : GameSystem<TigerDistractionTouristComponent>
    {
        public override void Register(TigerDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition.position));

            SystemUpdate(touristBrain)
                .Where(tourist => tourist.States.CurrentState.Value is Interacting)
                .Subscribe(InteractWithTiger(component))
                .AddToLifecycleOf(component);
        }

        private static Action<TouristBrainComponent> InteractWithTiger(TigerDistractionTouristComponent component)
        {
            return tourist =>
            {
                if (!component) return;

                component.LastDistractionProgressTime -= Time.deltaTime;

                if (component.LastDistractionProgressTime <= 0)
                {
                    component.DistractionProgress.Value = 1;
                    component.GetComponent<TouristBrainComponent>()
                        .States
                        .GoToState(new Dead(component));
                    Object.Destroy(component);
                }

                component.DistractionProgress.Value = 1 - component.LastDistractionProgressTime / component.MaxProgressTime;
            };
        }
    }
}
