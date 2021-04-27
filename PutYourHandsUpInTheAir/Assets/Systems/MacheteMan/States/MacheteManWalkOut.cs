using SystemBase.StateMachineBase;

namespace Systems.MacheteMan
{
    [NextValidStates(typeof(MacheteManOutOfLevel))]
    public class MacheteManWalkOut : BaseState<MacheteManComponent>
    {
        public override void Enter(StateContext<MacheteManComponent> context)
        {
        }
    }
}
