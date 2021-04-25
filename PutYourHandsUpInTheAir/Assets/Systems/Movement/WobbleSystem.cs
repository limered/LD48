using SystemBase;
using Systems.Movement.States;
using Systems.Tourist;
using UniRx;
using UnityEngine;
using Utils.Plugins;

namespace Systems.Movement
{
    public class WobbleSystem : GameSystem<RunWobbleComponent, MovementComponent>
    {
        public override void Register(RunWobbleComponent component)
        {
            component.States.Start(new NotWobbling());

            component.States.CurrentState
                .Where(state => state is Wobbling)
                .Subscribe(state =>
                {
                    //TODO: wobble
                })
                .AddToLifecycleOf(component);
        }

        public override void Register(MovementComponent component)
        {
            var wobble = component.GetComponent<RunWobbleComponent>();
            
            component.Direction
                .Subscribe(direction =>
                {
                    if(direction != Vector2.zero && wobble.States.CurrentState.Value is NotWobbling) wobble.States.GoToState(new Wobbling());
                    if(direction == Vector2.zero && wobble.States.CurrentState.Value is Wobbling) wobble.States.GoToState(new NotWobbling());
                    
                    
                })
                .AddToLifecycleOf(component);
        }
    }
}