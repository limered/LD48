using SystemBase.StateMachineBase;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateGoingToLocation), typeof(PlayerStateGoingToDistraction))]
    public class PlayerStateIdle : BaseState<PlayerComponent>
    {
        public override void Enter(StateContext<PlayerComponent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}