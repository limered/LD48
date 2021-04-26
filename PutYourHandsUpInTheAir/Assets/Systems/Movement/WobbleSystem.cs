using SystemBase;
using UniRx;
using UnityEngine;
using Utils.Plugins;

namespace Systems.Movement
{
    public class WobbleSystem : GameSystem<RunWobbleComponent, MovementComponent>
    {
        public override void Register(RunWobbleComponent component)
        {
            SystemUpdate()
                .Subscribe(_ =>
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
                    //TODO: update wobble
                })
                .AddToLifecycleOf(component);
        }
    }
}