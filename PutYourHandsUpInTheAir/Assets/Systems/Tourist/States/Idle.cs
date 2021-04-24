using SystemBase.StateMachineBase;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(PickingInterest), typeof(WalkingOutOfLevel))]
    public class Idle : BaseState<TouristBrainComponent>
    {
        public override void Enter(StateContext<TouristBrainComponent> context)
        {

        }
    }
}
