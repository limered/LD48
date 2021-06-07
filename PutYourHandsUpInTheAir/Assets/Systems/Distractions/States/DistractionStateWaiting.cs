using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Movement;
using UnityEngine;

namespace Systems.Distractions.States
{
    public class DistractionStateWaiting : BaseState<DistractionOriginComponent>
    {
        private DistractionSpawnerComponent _spawner;

        public DistractionStateWaiting(DistractionSpawnerComponent spawner)
        {
            _spawner = spawner;
        }

        public override void Enter(StateContext<DistractionOriginComponent> context)
        {
            Debug.Log("Wait");
            context.Owner.GetComponent<TwoDeeMovementComponent>().Stop();
        }
    }
}