using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Interacting), typeof(GoingBackToIdle), typeof(WalkingOutOfLevel))]
    public class GoingToAttraction : BaseState<TouristBrainComponent>
    {
        public Transform AttractionPosition { get; }

        public GoingToAttraction(Transform attractionPosition)
        {
            AttractionPosition = attractionPosition;
        }
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }
    }
}