using SystemBase.StateMachineBase;

namespace Systems.Movement.States
{
    [NextValidStates(typeof(NotWobbling))]
    public class Wobbling : BaseState<RunWobbleComponent>
    {
        public override void Enter(StateContext<RunWobbleComponent> context)
        {
            
        }
    }
}