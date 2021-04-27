using SystemBase.StateMachineBase;

namespace Systems.MacheteMan
{
    [NextValidStates(typeof(MacheteManPrepare))]
    public class MacheteManCreate : BaseState<MacheteManComponent>
    {
        public override void Enter(StateContext<MacheteManComponent> context)
        {
        }
    }
}
