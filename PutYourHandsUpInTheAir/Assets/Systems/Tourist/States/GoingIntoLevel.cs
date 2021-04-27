using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Idle), typeof(GoingBackToIdle), typeof(WalkingOutOfLevel))]
    public class GoingIntoLevel : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {

        }
    }
}