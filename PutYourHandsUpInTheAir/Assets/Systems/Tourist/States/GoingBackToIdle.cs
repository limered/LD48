using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(Idle), typeof(WalkingOutOfLevel))]
    public class GoingBackToIdle : BaseState<TouristBrainComponent>
    {
        /// They will try to gather around that point
        public Vector2 IdlePosition { get; }

        public GoingBackToIdle(Vector2 idlePosition)
        {
            IdlePosition = idlePosition;
        }
        
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            
        }

        public override string ToString()
        {
            return $"GoingBackToIdle({IdlePosition})";
        }
    }
}