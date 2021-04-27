using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates()]
    public class WalkedOut : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }
    }
}