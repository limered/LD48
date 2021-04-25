﻿using SystemBase;
using Systems.DistractionControl;
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
    public class TigerDistractionSystem : GameSystem<TigerDistractionTouristComponent>
    {
        private float _distractionTime = 3;

        public override void Register(TigerDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(Vector3.zero.RandomVector()));
            touristBrain.States.CurrentState
                .Where(state => state is Interacting)
                .Subscribe(_ => StartInteracting(component))
                .AddTo(component);
        }

        private void StartInteracting(TigerDistractionTouristComponent component)
        {
            component.LastDistractionTime = _distractionTime;

            component.UpdateAsObservable()
                .Where(_ => component)
                .Subscribe(_ => UpdateTimer(component))
                .AddTo(component);
        }

        private void UpdateTimer(TigerDistractionTouristComponent comp)
        {
            comp.LastDistractionTime -= Time.deltaTime;

            if (comp.LastDistractionTime <= 0)
            {
                comp.DistractionProgress.Value = 1;
                comp.GetComponent<TouristBrainComponent>()
                    .States
                    .GoToState(new GoingBackToIdle());
                Object.Destroy(comp);
            }

            comp.DistractionProgress.Value = 1 - comp.LastDistractionTime / _distractionTime;
        }
    }
}
