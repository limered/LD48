using SystemBase.StateMachineBase;

namespace Systems.MacheteMan
{
    [NextValidStates(typeof(MacheteManClearing))]
    public class MacheteManPrepare : BaseState<MacheteManComponent>
    {
        public override void Enter(StateContext<MacheteManComponent> context)
        {
        }
    }
}
