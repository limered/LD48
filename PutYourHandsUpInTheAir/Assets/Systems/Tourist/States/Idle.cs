using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(PickingInterest), typeof(WalkingOutOfLevel), typeof(GoingBackToIdle),
        /*just for testing*/typeof(Dead) /*just for testing*/)]
    public class Idle : BaseState<TouristBrainComponent>
    {
        /// They will try to gather around that point
        public Vector2 IdlePosition { get; }

        public Idle(Vector2 idlePosition)
        {
            IdlePosition = idlePosition;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
        }

        public override string ToString()
        {
            return $"{nameof(Idle)}({IdlePosition})";
        }
    }
}