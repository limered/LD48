using SystemBase.StateMachineBase;

namespace Systems.Movement.States
{
    [NextValidStates(typeof(Wobbling))]
    public class NotWobbling : BaseState<RunWobbleComponent>
    {
        public override void Enter(StateContext<RunWobbleComponent> context)
        {
        }
    }
}