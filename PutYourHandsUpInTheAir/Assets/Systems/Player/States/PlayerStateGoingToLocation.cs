using SystemBase.StateMachineBase;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateIdle))]
    public class PlayerStateGoingToLocation : BaseState<PlayerComponent>
    {
        public override void Enter(StateContext<PlayerComponent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}