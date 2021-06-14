using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Distractions.States
{
    public class DistractionStateAborted : BaseState<DistractionOriginComponent>
    {
        private readonly DistractionSpawnerComponent _spawner;
        public DistractionStateAborted(DistractionSpawnerComponent spawner)
        {
            _spawner = spawner;
        }

        public override void Enter(StateContext<DistractionOriginComponent> context)
        {
            var movement = context.Owner.GetComponent<TwoDeeMovementComponent>();
            context.Owner.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    var distance = _spawner.DespawnPosition.transform.position - context.Owner.transform.position;
                    if (distance.magnitude < 0.03)
                    {
                        Object.Destroy(context.Owner);
                    }
                    else
                    {
                        movement.Direction.Value = distance.normalized;
                    }
                })
                .AddTo(this);
        }
    }
}
