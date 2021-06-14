using SystemBase;
using Systems.Movement;
using Systems.Player.States;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    [GameSystem(typeof(TwoDeeMovementSystem))]
    public class PlayerControlSystem : GameSystem<PlayerComponent>
    {
        public override void Register(PlayerComponent component)
        {
            component.State.Start(new PlayerStateIdle());

            component.State.CurrentState.Subscribe(Debug.Log).AddTo(component);
        }
    }
}