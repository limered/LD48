using System;
using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Movement;
using Systems.Tourist.States;
using Assets.Utils.Math;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Systems.Distractions;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristBrainSystem : GameSystem<TouristBrainComponent>
    {

        public override void Register(TouristBrainComponent component)
        {
            var movement = component.GetComponent<TwoDeeMovementComponent>();

            component.States.CurrentState
                // .LogOnNext(state => $"{component.touristName}: {state}")
                .Do(state => component.debugCurrentState = $"{state}")
                .Subscribe(state =>
                {
                    if (state is GoingIntoLevel) //TODO: do we need this state?
                    {
                        //this prevents a deadlock when setting the state directly
                        Observable.Timer(TimeSpan.FromSeconds(0))
                            .Subscribe(_ =>
                                component.States.GoToState(
                                    new GoingBackToIdle(Random.insideUnitCircle /*<- gather around this point*/)))
                            .AddTo(state);
                    }
                    else if (state is GoingBackToIdle goingIdle)
                    {
                        GoingToIdlePosition(goingIdle, component, movement);
                    }
                    else if (state is Idle idle)
                    {
                        Idle(idle, component, movement);
                    }
                    else if (state is PickingInterest)
                    {
                        //TODO: show some kind of thinking process (DistractionControlSystem)
                    }
                    else if (state is GoingToAttraction attraction)
                    {
                        GoingToAttraction(attraction, component, movement);
                    }
                    else if (state is Interacting interacting)
                    {
                        Interacting(interacting, component, movement);
                    }
                    else if (state is Dead dead)
                    {
                        PersonDies(component, movement, dead);
                    }
                    else if (state is WalkingOutOfLevel walkOut)
                    {
                        SystemUpdate()
                            .Subscribe(_ =>
                            {
                                movement.Direction.Value =
                                    (walkOut.Target.position - component.transform.position).normalized;
                                movement.VelocityCutoff = component.normalSpeed;
                            })
                            .AddToLifecycleOf(component);
                    }
                    else if (state is WalkedOut)
                    {
                        movement.Direction.Value = Vector2.zero;
                    }
                })
                .AddToLifecycleOf(component);
        }

        private void PersonDies(TouristBrainComponent component, TwoDeeMovementComponent movement, Dead dead)
        {
            MessageBroker.Default.Publish(new ShowDeadPersonAction
            {
                TouristName = component.touristName.Value,
                TouristFaceIndex = component.headPartIndex.Value,
                DistractionIndex = GetDistractionIndex(dead.Distraction)
            });

            movement.Direction.Value = Vector2.zero;
            if (component.GetComponent<Collider>()) Object.Destroy(component.GetComponent<Collider>());
            var body = component.GetComponent<TouristBodyComponent>();
            if (body)
            {
                body.blood.SetActive(true);
                body.livingBody.transform.Rotate(new Vector3(0, 0, 1), 360 * Random.value);
            }
        }

        private int GetDistractionIndex(DistractedTouristComponent distraction)
        {
            if(distraction is TigerDistractionTouristComponent)
            {
                return 0;
            } else
            {
                return 1;
            }
        }

        private void GoingToIdlePosition(GoingBackToIdle state, TouristBrainComponent tourist,
            TwoDeeMovementComponent movement)
        {
            SystemUpdate()
                .Select(_ => state.IdlePosition - (Vector2) tourist.transform.position)
                .Subscribe(delta =>
                {
                    if (state.IdlePosition != Vector2.zero && delta.magnitude < 0.1f) //magic epsilon distance
                    {
                        tourist.States.GoToState(new Idle(state.IdlePosition));
                    }
                    else if (state.IdlePosition == Vector2.zero && delta.magnitude < 1.3f) //magic distance around center that I define as idle-zone
                    {
                        tourist.States.GoToState(new Idle(state.IdlePosition));
                    }
                    else
                    {
                        movement.VelocityCutoff = tourist.normalSpeed;
                        movement.Direction.Value = delta.normalized;
                    }
                })
                .AddTo(state);
        }

        private void Idle(Idle state, TouristBrainComponent tourist, TwoDeeMovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero;

            Observable.Interval(TimeSpan.FromSeconds(tourist.idleMinTimeWithoutMovementInSeconds))
                .Subscribe(_ =>
                {
                    var stop = Random.value * 10;
                    if (stop < 6)
                    {
                        movement.Direction.Value = Vector2.zero;
                    }
                    else
                    {
                        var pos = state.IdlePosition + Random.insideUnitCircle;
                        var rndMovement = (pos - (Vector2) movement.transform.position)
                            .Rotate(Random.Range(-90, 90))
                            .normalized;
                        var delta = movement.Direction.Value + rndMovement;
                        movement.Direction.Value = delta.normalized;
                    }

                    movement.VelocityCutoff = tourist.idleSpeed;
                })
                .AddTo(state);

            // TODO: talking to each other 
        }

        private void GoingToAttraction(GoingToAttraction attraction, TouristBrainComponent tourist,
            TwoDeeMovementComponent movement)
        {
            SystemUpdate()
                .Select(_ => attraction.AttractionPosition - (Vector2) tourist.transform.position)
                .Do(delta => tourist.debugTargetDistance = delta)
                .Subscribe(delta =>
                {
                    if (delta.magnitude < 0.1f)
                    {
                        tourist.States.GoToState(new Interacting());
                    }
                    else
                    {
                        movement.Direction.Value =
                            (attraction.AttractionPosition - (Vector2) tourist.transform.position).normalized;
                        movement.VelocityCutoff = tourist.attractedSpeed;
                    }
                })
                .AddTo(attraction);
        }

        private void Interacting(Interacting interacting, TouristBrainComponent tourist, TwoDeeMovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero; 
        }
    }
}