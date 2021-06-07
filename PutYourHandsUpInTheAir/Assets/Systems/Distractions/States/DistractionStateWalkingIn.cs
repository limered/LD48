using SystemBase.StateMachineBase;
using Systems.DistractionManagement;

namespace Systems.Distractions.States
{
    public class DistractionStateWalkingIn : BaseState<DistractionOriginComponent>
    {
        private DistractionSpawnerComponent _spawner;
        public DistractionStateWalkingIn(DistractionSpawnerComponent component)
        {
            _spawner = component;
        }

        public override void Enter(StateContext<DistractionOriginComponent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}