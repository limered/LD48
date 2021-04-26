using UnityEngine;
using SystemBase;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using Utils.Plugins;
using Systems.Movement;
using System.Linq;
using Systems.DistractionControl;

[GameSystem]
public class BusDistractionSystem : GameSystem<BusDistractionTouristComponent, BusComponent>
{
    public override void Register(BusComponent bus)
    {
        SystemUpdate()
            .Where(_ => !bus.enteredScene)
            .Subscribe(_ => {
                var tourists = GameObject.FindGameObjectsWithTag("tourist");
                if(!tourists.Any())
                {
                    return;
                }

                var allPaid = tourists.All(t => {
                    var touristComp = t.GetComponent<TouristBrainComponent>();
                    return touristComp && touristComp.HasPaid;
                }
                );
                if (allPaid)
                {
                    bus.GetComponentInParent<DistractionComponent>().enabled = true;
                    bus.enteredScene = true;
                    var movementComp = bus.GetComponent<MovementComponent>();
                    movementComp.Direction.Value = bus.StartPosititon - (Vector2)bus.transform.position;
                }
            })
            .AddTo(bus);

        SystemUpdate()
            .Where(_ => bus.enteredScene && !bus.leftScene)
            .Subscribe(_ =>
            {
                if(bus.transform.position.x > bus.StartPosititon.x)
                {
                    return;
                }
                bus.leftScene = true;
                bus.transform.position = bus.StartPosititon;
                var movementComp = bus.GetComponent<MovementComponent>();
                movementComp.Direction.Value = Vector2.zero;
            });
    }

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

        var bus = GameObject.FindGameObjectWithTag("bus");
        var busComponent = bus.GetComponent<BusComponent>();
        var movementComponent = bus.GetComponent<MovementComponent>();
        touristBrain.transform.parent = bus.transform;

        if (component.LastDistractionProgressTime <= 0)
        {
            component.DistractionProgress.Value = 1;
            movementComponent.Direction.Value = busComponent.EndPosititon - (Vector2) component.transform.position;
        }

        component.DistractionProgress.Value = 1 - component.LastDistractionProgressTime / component.MaxProgressTime;
    }
}
