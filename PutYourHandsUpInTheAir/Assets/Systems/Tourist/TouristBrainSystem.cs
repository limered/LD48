using System.Collections.Generic;
using SystemBase;
using Systems.Movement;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristBrainSystem : GameSystem<TouristBrainComponent>
    {
        public override void Register(TouristBrainComponent component)
        {
            component.States.Start(new GoingIntoLevel());
            var movement = component.GetComponent<MovementComponent>();

            component.States.CurrentState
                .Subscribe(state =>
                {
                    // if (state is FollowingGuide)
                    // {
                    //     MoveAlongGuide(component, movement);
                    // }
                    // else if (state is Distracted distracted)
                    // {
                    //     MoveTowardAttraction(component, distracted.By, movement);
                    // }
                    // else if (state is Interacting interacting)
                    // {
                    //     Interacting(component, interacting.With, movement);
                    // }
                })
                .AddTo(component);
        }

        private void Idle(TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero;
            // TODO: idle "wusel" movement
            // TODO: talking to each other 
        }

        private void GoingToAttraction(TouristBrainComponent tourist, MovementComponent movement)
        {
            //var delta = //TODO: need delta to attraction here
            // movement.Direction.Value = delta.normalized;
        }

        private void Interacting(TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero; // TODO: stop movement here?
        }
    }
}