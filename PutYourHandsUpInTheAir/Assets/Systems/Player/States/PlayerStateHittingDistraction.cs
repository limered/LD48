using SystemBase.StateMachineBase;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateGoingToLocation), typeof(PlayerStateGoingToLocation), typeof(PlayerStateIdle))]
    public class PlayerStateHittingDistraction : BaseState<PlayerComponent>
    {
        public override void Enter(StateContext<PlayerComponent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}