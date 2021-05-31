using SystemBase.StateMachineBase;
using Systems.Distractions;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(/*none, you are dead...*/)]
    public class Dead : BaseState<TouristBrainComponent>
    {
        public DistractedTouristComponent Distraction;

        public Dead(DistractedTouristComponent distraction)
        {
            Distraction = distraction;
        }
        public override void Enter(StateContext<TouristBrainComponent> context)
        {

        }
    }
}