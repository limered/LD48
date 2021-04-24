using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(FollowingGuide))]
    public class Contemplating : BaseState<TouristMovementComponent>
    {
        public AttractionComponent Attraction { get; set; }
        public override void Enter(StateContext<TouristMovementComponent> context)
        {
            
        }
    }
}
