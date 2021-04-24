using SystemBase.StateMachineBase;
using Systems;
using Systems.Tourist;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Tourist
{
    [NextValidStates(typeof(Distracted))]
    public class FollowingGuide : BaseState<TouristMovementComponent>
    {
        public override void Enter(StateContext<TouristMovementComponent> context)
        {

        }
    }
}
