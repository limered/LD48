using SystemBase.StateMachineBase;
using Systems.Movement;

namespace Systems.Tourist.States
{
    [NextValidStates()]
    public class WalkedOut : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            context.Owner.GetComponent<TwoDeeMovementComponent>().Stop();
        }
    }
}