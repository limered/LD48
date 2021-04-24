using System;
using System.Collections.Generic;
using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Movement;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;
using Random = UnityEngine.Random;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristBrainSystem : GameSystem<TouristBrainComponent>
    {
        public override void Register(TouristBrainComponent component)
        {
            component.tag = "tourist";
            if (string.IsNullOrWhiteSpace(component.touristName))
                component.touristName = TouristNames.All[Random.Range(0, TouristNames.All.Length)];

            component.States.Start(new GoingIntoLevel());

            var movement = component.GetComponent<MovementComponent>();

            component.States.CurrentState
                .LogOnNext(state => $"{component.touristName}: {state}")
                .Do(state => component.debugCurrentState = $"{state}")
                .Subscribe(state =>
                {
                    if (state is GoingIntoLevel || state is GoingBackToIdle)
                    {
                        GoingToIdlePosition(state, component, movement);
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
                    else if (state is Dead)
                    {
                        //TODO: rotting animation
                    }
                    else if (state is WalkingOutOfLevel)
                    {
                        //TODO: walk to the top part (Exit Zone) of the level 
                    }
                })
                .AddTo(component);
        }

        private void GoingToIdlePosition(BaseState<TouristBrainComponent> state, TouristBrainComponent tourist,
            MovementComponent movement)
        {
            var center = Vector2.zero;
            movement.Direction.Value = -tourist.transform.position.normalized;

            SystemUpdate()
                .Select(_ => center - (Vector2) tourist.transform.position)
                .TakeWhile(delta => delta.magnitude > 0.1f)
                .Subscribe(_ => { }, () => { tourist.States.GoToState(new Idle()); })
                .AddTo(state.StateDisposables);
        }

        private void Idle(Idle state, TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero;

            if (tourist.collider)
            {
                // tourist.collider.OnCollisionStayAsObservable()
                //     .Delay(TimeSpan.FromSeconds(Random.Range(0, tourist.brainDelayInSeconds)))
                //     .TakeUntil(state)
                //     .Subscribe();
            }

            // SystemUpdate()
            //     .Where(_ => tourist.collider != null)
            //     .Select(_ => tourist.collider)
            //     .Subscribe(myCollider =>
            //     {
            //         myCollider
            //     })
            //     .AddTo(state.StateDisposables);
            // TODO: idle "wusel" movement
            // TODO: talking to each other 
        }

        private void GoingToAttraction(GoingToAttraction attraction, TouristBrainComponent tourist,
            MovementComponent movement)
        {
            movement.Direction.Value =
                (attraction.AttractionPosition - (Vector2) tourist.transform.position).normalized;

            SystemUpdate()
                .Select(_ => attraction.AttractionPosition - (Vector2) tourist.transform.position)
                .TakeWhile(delta => delta.magnitude > 0.1f)
                .Subscribe(_ => { }, () => { tourist.States.GoToState(new Interacting()); })
                .AddTo(attraction.StateDisposables);
        }

        private void Interacting(Interacting interacting, TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero; // TODO: just stop movement here?
        }
    }
}