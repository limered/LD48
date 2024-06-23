using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist.States
{
    public class WaitingForTheBus : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
        }
    }
}
