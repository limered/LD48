using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Distracted))]
    public class FollowingGuide : BaseState<TouristMovementComponent>
    {
        public override void Enter(StateContext<TouristMovementComponent> context)
        {

        }
    }
}
