using System;
using System.Collections;
using System.Collections.Generic;
using SystemBase;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;

[GameSystem]
public class MoneyDistractionSystem : GameSystem<MoneyDistractionTouristComponent>
{
    public override void Register(MoneyDistractionTouristComponent component)
    {
        var touristBrain = component.GetComponent<TouristBrainComponent>();
        touristBrain.States.GoToState(new GoingToAttraction(touristBrain.transform.position));
        touristBrain.States.CurrentState
            .Where(state => state is Interacting)
            .Subscribe(_ => StartInteracting(component))
            .AddToLifecycleOf(component);
    }

    private static void StartInteracting(MoneyDistractionTouristComponent component)
    {
        component.UpdateAsObservable()
            .Where(_ => component)
            .Subscribe(_ => UpdateTimer(component))
            .AddToLifecycleOf(component);
    }

    private static void UpdateTimer(MoneyDistractionTouristComponent comp)
    {
        comp.LastDistractionProgressTime -= Time.deltaTime;

        if (comp.LastDistractionProgressTime <= 0)
        {
            comp.DistractionProgress.Value = 1;
            comp.GetComponent<TouristBrainComponent>()
                .States
                .GoToState(new GoingBackToIdle(Vector2.zero));
        }

        comp.DistractionProgress.Value = 1 - comp.LastDistractionProgressTime / comp.MaxProgressTime;
    }
}
