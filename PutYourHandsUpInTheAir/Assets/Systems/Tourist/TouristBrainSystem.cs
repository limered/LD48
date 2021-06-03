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
using Systems.Room.Events;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristBrainSystem : GameSystem<TouristBrainComponent>
    {
        public override void Register(TouristBrainComponent component)
        {
            MessageBroker.Default.Receive<RoomTimerEndedAction>()
                .Subscribe(msg => component.States.GoToState(new WalkingOutOfLevel(msg.WalkOutPosition)))
                .AddTo(component);


            var movement = component.GetComponent<TwoDeeMovementComponent>();

            component.States.CurrentState
                // .LogOnNext(state => $"{component.touristName}: {state}")
                .Do(state => component.debugCurrentState = $"{state}")
                .Subscribe(state =>
                {
                    switch (state)
                    {
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
                                })
                                .AddToLifecycleOf(component);
                            break;
                        case WalkedOut _:
                            movement.Stop();
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
                DistractionType = dead.KilledByDistractionType,
            });

            MessageBroker.Default.Publish(new ReducePotentialIncomeAction
            {
                IncomeVanished = 100
            });

            movement.Stop();
            if (component.GetComponent<Collider>()) Object.Destroy(component.GetComponent<Collider>());
            var body = component.GetComponent<TouristBodyComponent>();
            if (body)
            {
                body.blood.SetActive(true);
                body.livingBody.transform.Rotate(new Vector3(0, 0, 1), 360 * Random.value);
            }
        }

        private void GoingToIdlePosition(GoingBackToIdle state, TouristBrainComponent tourist,
            TwoDeeMovementComponent movement)
        {
            SystemUpdate()
                .Select(_ => state.GatherPosition - (Vector2) tourist.transform.position)
                .Subscribe(delta =>
                {
                    if (state.GatherPosition != Vector2.zero && delta.magnitude < 0.1f) //magic epsilon distance
                    {
                        tourist.States.GoToState(new Idle(state.GatherPosition, tourist));
                    }
                    else if (state.GatherPosition == Vector2.zero && delta.magnitude < 1.3f) //magic distance around center that I define as idle-zone
                    {
                        tourist.States.GoToState(new Idle(state.GatherPosition, tourist));
                    }
                    else
                    {
                        movement.Direction.Value = delta.normalized;
                    }
                })
                .AddTo(state);
        }

        private void GoingToAttraction(GoingToAttraction attraction, TouristBrainComponent tourist,
            TwoDeeMovementComponent movement)
        {
            SystemUpdate()
                .Select(_ => (Vector2) attraction.AttractionPosition.position - (Vector2) tourist.transform.position)
                .Subscribe(delta =>
                {
                    if (TouristReachedDistraction(delta))
                    {
                        tourist.States.GoToState(new Interacting());
                    }
                    else
                    {
                        movement.Direction.Value = 
                            ((Vector2) attraction.AttractionPosition.position - (Vector2) tourist.transform.position).normalized;
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
            movement.SlowStop();
        }
    }
}