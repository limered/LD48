using SystemBase.StateMachineBase;

namespace Systems.MacheteMan
{
    [NextValidStates(typeof(MacheteManWalkOut))]
    public class MacheteManClearing : BaseState<MacheteManComponent>
    {
        public override void Enter(StateContext<MacheteManComponent> context)
        {
        }
    }
}
