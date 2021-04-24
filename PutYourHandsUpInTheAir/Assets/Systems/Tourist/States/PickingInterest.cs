using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(GoingToAttraction))]
    public class PickingInterest : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }
    }
}
