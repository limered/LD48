using SystemBase;
using Systems.Distractions;
using Systems.Player;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[GameSystem]
public class MoneyDistractionSystem : GameSystem<MoneyDistractionTouristComponent, PlayerComponent>
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

    public override void Register(PlayerComponent component)
    {
        RegisterWaitable(component);
    }

    private void StartInteracting(MoneyDistractionTouristComponent component)
    {
        component.UpdateAsObservable()
            .Where(_ => component && component.IsPaying)
            .Subscribe(_ => UpdateTimer(component))
            .AddToLifecycleOf(component);

        WaitOn<PlayerComponent>()
                .Subscribe(player => StartPlayerCollisionTracking(component, player))
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


    private void StartPlayerCollisionTracking(MoneyDistractionTouristComponent component,
            PlayerComponent player)
    {
        player.OnTriggerEnterAsObservable()
            .Subscribe(coll => CollideWithPlayer(coll, component))
            .AddToLifecycleOf(component);
    }

    private void CollideWithPlayer(Collider coll, MoneyDistractionTouristComponent component)
    {
        var tourist = coll.gameObject.GetComponent<TouristBrainComponent>();
        if (tourist)
        {
            component.HasPaid = true;
            tourist.States
                .GoToState(new GoingBackToIdle(Random.insideUnitCircle));
        }
    }
}
