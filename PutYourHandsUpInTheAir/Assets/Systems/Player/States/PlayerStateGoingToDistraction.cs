using SystemBase.StateMachineBase;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateGoingToLocation), typeof(PlayerStateHittingDistraction))]
    public class PlayerStateGoingToDistraction : BaseState<PlayerComponent>
    {
        public override void Enter(StateContext<PlayerComponent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
