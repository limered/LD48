using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates()]
    public class WalkingOutOfLevel : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {

        }
    }
}