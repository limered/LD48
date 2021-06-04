using System;
using System.Linq;
using SystemBase;
using Systems.DistractionControl;
using Systems.Movement;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Plugins;

namespace Systems.Distractions.ForEnd
{
    [GameSystem]
    public class BusDistractionSystem : GameSystem<BusDistractionTouristComponent, BusComponent>
    {
        public override void Register(BusComponent bus)
        {
            Observable.Timer(TimeSpan.FromSeconds(bus.DriveInTime))
                .Subscribe(_ => SpawnBus(bus))
                .AddToLifecycleOf(bus);


            SystemUpdate()
                .Where(_ => bus.enteredScene && !bus.leftScene)
                .Subscribe(_ =>
                {
                    if(bus.transform.position.x > bus.StartPosititon.x)
                    {
                        return;
                    }
                    GameObject[] tourists = GameObject.FindGameObjectsWithTag("tourist");
                    if (!tourists.Any())
                    {
                        return;
                    }
                    AddBussDistractionToAllPeople(tourists, bus.GetComponentInParent<DistractionOriginComponent>());
                    bus.leftScene = true;
                    bus.transform.position = bus.StartPosititon;
                    var movementComp = bus.GetComponent<TwoDeeMovementComponent>();
                    movementComp.Direction.Value = Vector2.zero;
                }).AddTo(bus);

            SystemUpdate()
                .Where(_ => bus.leftScene)
                .Subscribe(_ => {
                    if (bus.transform.position.x < bus.EndPosititon.x)
                    {
                        SceneManager.LoadScene("AdvancedEndScene");
                    }
                }).AddTo(bus);
        }

        private void SpawnBus(BusComponent bus)
        {
            bus.enteredScene = true;
            var movementComp = bus.GetComponent<TwoDeeMovementComponent>();
            movementComp.Direction.Value = bus.StartPosititon - (Vector2)bus.transform.position;
        }

        private void AddBussDistractionToAllPeople(GameObject[] tourists, DistractionOriginComponent distractionOrigin)
        {
            foreach (var tourist in tourists.Select(t => t.GetComponent<TouristBrainComponent>()))
            {
                //tourist.StateContext.GoToState(new PickingInterest());

                var busComp = tourist.AddComponent<BusDistractionTouristComponent>();
                busComp.BusStopPosition = distractionOrigin.GetComponent<BusComponent>().StartPosititon;
                busComp.CreatedFrom = distractionOrigin;
                busComp.LastDistractionProgressTime = distractionOrigin.DistractionInteractionDuration;
            }
        }

        public override void Register(BusDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
        
            //touristBrain.StateContext.GoToState(new GoingToDistraction(component.BusStopPosition));
            touristBrain.StateContext.CurrentState
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
            var movementComponent = bus.GetComponent<TwoDeeMovementComponent>();
            touristBrain.transform.parent = bus.transform;

            if (component.LastDistractionProgressTime <= 0)
            {
                component.DistractionProgress.Value = 1;
                movementComponent.Direction.Value = busComponent.EndPosititon - (Vector2) component.transform.position;
            }

            component.DistractionProgress.Value = 1 - component.LastDistractionProgressTime / component.MaxProgressTime;
        }
    }
}
