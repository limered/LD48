using SystemBase.StateMachineBase;
using Systems;
using Systems.Tourist;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Tourist
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
