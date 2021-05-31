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
using Systems.GameMessages.Messages;

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
                    switch (state)
                    {
                        //TODO: do we need this state?
                        case GoingIntoLevel _:
                            //this prevents a deadlock when setting the state directly
                            Observable.Timer(TimeSpan.FromSeconds(0))
                                .Subscribe(_ =>
                                    component.States.GoToState(
                                        new GoingBackToIdle(Random.insideUnitCircle /*<- gather around this point*/)))
                                .AddTo(state);
                            break;
                        case GoingBackToIdle goingIdle:
                            GoingToIdlePosition(goingIdle, component, movement);
                            break;
                        case Idle idle:
                            Idle(idle, component, movement);
                            break;
                        case PickingInterest _:
                            //TODO: show some kind of thinking process (DistractionControlSystem)
                            break;
                        case GoingToAttraction attraction:
                            GoingToAttraction(attraction, component, movement);
                            break;
                        case Interacting interacting:
                            Interacting(movement);
                            break;
                        case Dead dead:
                            PersonDies(component, movement, dead);
                            break;
                        case WalkingOutOfLevel walkOut:
                            SystemUpdate()
                                .Subscribe(_ =>
                                {
                                    movement.Direction.Value =
                                        (walkOut.Target.position - component.transform.position).normalized;
                                    movement.MaxVelocity = component.normalSpeed;
                                })
                                .AddToLifecycleOf(component);
                            break;
                        case WalkedOut _:
                            movement.Direction.Value = Vector2.zero;
                            break;
                    }
                })
                .AddToLifecycleOf(component);
        }

        private void PersonDies(TouristBrainComponent component, TwoDeeMovementComponent movement, Dead dead)
        {
            MessageBroker.Default.Publish(new ShowDeadPersonMessageAction
            {
                TouristName = component.touristName.Value,
                TouristFaceIndex = component.headPartIndex.Value,
                DistractionIndex = GetDistractionIndex(dead.Distraction)
            });

            MessageBroker.Default.Publish(new ReducePotentialIncomeAction
            {
                IncomeVanished = 100
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
            return distraction is TigerDistractionTouristComponent
                ? 0 
                : 1;
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
                        movement.MaxVelocity = tourist.normalSpeed;
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

                    movement.MaxVelocity = tourist.idleSpeed;
                })
                .AddTo(state);

            // TODO: talking to each other 
        }

        private void GoingToAttraction(GoingToAttraction attraction, TouristBrainComponent tourist,
            TwoDeeMovementComponent movement)
        {
            SystemUpdate()
                .Select(_ => attraction.AttractionPosition - (Vector2) tourist.transform.position)
                .Subscribe(delta =>
                {
                    if (TouristReachedDistraction(delta))
                    {
                        tourist.States.GoToState(new Interacting());
                    }
                    else
                    {
                        movement.Direction.Value = 
                            (attraction.AttractionPosition - (Vector2) tourist.transform.position).normalized;
                    }
                })
                .AddTo(attraction);
        }

        private static bool TouristReachedDistraction(Vector2 delta)
        {
            return delta.magnitude < 0.1f;
        }

        private void Interacting(TwoDeeMovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero;
        }
    }
}