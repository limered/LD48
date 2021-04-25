using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Interacting), typeof(GoingBackToIdle))]
    public class GoingToAttraction : BaseState<TouristBrainComponent>
    {
        public Vector2 AttractionPosition { get; }

        public GoingToAttraction(Vector2 attractionPosition)
        {
            AttractionPosition = attractionPosition;
        }
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }
    }
}