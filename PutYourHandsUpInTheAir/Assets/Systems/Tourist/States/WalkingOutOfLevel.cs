using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates()]
    public class WalkingOutOfLevel : BaseState<TouristBrainComponent>
    {
        public Transform Target { get; private set; }

        public WalkingOutOfLevel(Transform transform)
        {
            Target = transform;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {

        }
    }
}