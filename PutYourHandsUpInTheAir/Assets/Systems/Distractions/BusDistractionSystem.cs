using UnityEngine;
using SystemBase;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using Utils.Plugins;
using Systems.Movement;

[GameSystem]
public class BusDistractionSystem : GameSystem<BusDistractionTouristComponent>
{
    public override void Register(BusDistractionTouristComponent component)
    {
        var touristBrain = component.GetComponent<TouristBrainComponent>();
        touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition.position));
        touristBrain.States.CurrentState
            .Where(state => state is Interacting)
            .Subscribe(_ => StartInteracting(component, touristBrain))
            .AddToLifecycleOf(component);
    }

    private static void StartInteracting(BusDistractionTouristComponent component, TouristBrainComponent touristBrain)
    {
        component.UpdateAsObservable()
            .Where(_ => component)
            .Subscribe(_ => DriveAway(component, touristBrain))
            .AddToLifecycleOf(component);
    }

    private static void DriveAway(BusDistractionTouristComponent component, TouristBrainComponent touristBrain)
    {
        component.LastDistractionProgressTime -= Time.deltaTime;

        var bus = GameObject.FindGameObjectWithTag("bus").GetComponent<MovementComponent>();
        touristBrain.transform.parent = bus.transform;

        if (component.LastDistractionProgressTime <= 0)
        {
            component.DistractionProgress.Value = 1;
            bus.Direction.Value = component.EndPosititon - (Vector2) component.transform.position;
        }

        component.DistractionProgress.Value = 1 - component.LastDistractionProgressTime / component.MaxProgressTime;
    }
}
