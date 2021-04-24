using System.Collections.Generic;
using SystemBase;
using Systems.Movement;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;

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
                .Subscribe(state =>
                {
                    if (state is GoingIntoLevel || state is GoingBackToIdle)
                    {
                        GoingToIdlePosition(component, movement);
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

        private void GoingToIdlePosition(TouristBrainComponent tourist, MovementComponent movement)
        {
            var deltaToCenter = -tourist.transform.position; //(0,0,0) is level center
            movement.Direction.Value = deltaToCenter.normalized;
        }

        private void Idle(Idle state, TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero;
            // TODO: idle "wusel" movement
            // TODO: talking to each other 
        }

        private void GoingToAttraction(GoingToAttraction attraction, TouristBrainComponent tourist,
            MovementComponent movement)
        {
            var delta = attraction.AttractionPosition - (Vector2)tourist.transform.position;
            movement.Direction.Value = delta.normalized;

            // SystemUpdate()
            //     .TakeWhile(_ => delta.magnitude > 0.1f)
            //     .Subscribe(_ => {}, () => { tourist.States.CurrentState. })
            //     .AddTo(attraction.StateDisposables);
        }

        private void Interacting(Interacting interacting, TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero; // TODO: stop movement here?
        }
    }
}