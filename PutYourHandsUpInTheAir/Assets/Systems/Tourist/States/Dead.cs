using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(/*none, you are dead...*/)]
    public class Dead : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }
    }
}