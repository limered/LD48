using SystemBase.StateMachineBase;
using Systems.DistractionManagement;

namespace Systems.Distractions.States
{
    public class DistractionStateAborted : BaseState<DistractionOriginComponent>
    {
        public override void Enter(StateContext<DistractionOriginComponent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
