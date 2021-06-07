using SystemBase.StateMachineBase;
using Systems.Movement;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(GoingBackToIdle), typeof(Dead), typeof(WalkingOutOfLevel))]
    public class Interacting : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            context.Owner.GetComponent<TwoDeeMovementComponent>().SlowStop();
        }
    }
}