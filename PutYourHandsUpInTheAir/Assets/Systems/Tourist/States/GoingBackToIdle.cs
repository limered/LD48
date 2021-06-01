using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Idle), typeof(WalkingOutOfLevel), typeof(GoingToAttraction))]
    public class GoingBackToIdle : BaseState<TouristBrainComponent>
    {
        public Vector2 GatherPosition { get; } = Vector2.zero;

        public GoingBackToIdle() {}

        public GoingBackToIdle(Vector2 gatherPosition)
        {
            GatherPosition = gatherPosition;
        }
        
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }

        public override string ToString()
        {
            return $"GoingBackToIdle({GatherPosition})";
        }
    }
}