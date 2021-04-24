using SystemBase.StateMachineBase;
using Systems;
using Systems.Tourist;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Tourist
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
