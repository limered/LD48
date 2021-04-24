using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(FollowingGuide), typeof(Contemplating))]
    public class Distracted : BaseState<TouristMovementComponent>
    {
        public AttractionComponent MovingToward { get; set; }

        public override void Enter(StateContext<TouristMovementComponent> context)
        {
   
        }
    }
}
