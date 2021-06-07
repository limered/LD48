using System;
using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;

namespace Systems.Distractions.States
{
    [NextValidStates(typeof(DistractionStateWaiting), typeof(DistractionStateAborted))]
    public class DistractionStateWalkingIn : BaseState<DistractionOriginComponent>
    {
        private readonly DistractionSpawnerComponent _spawner;
        public DistractionStateWalkingIn(DistractionSpawnerComponent component)
        {
            _spawner = component;
        }

        public override void Enter(StateContext<DistractionOriginComponent> context)
        {
            context.Owner.UpdateAsObservable()
                .Subscribe(MoveToWaitPosition(context))
                .AddTo(this);
        }

        private Action<Unit> MoveToWaitPosition(StateContext<DistractionOriginComponent> context)
        {
            return _ =>
            {
                var waitPosition = _spawner.WaitPosition.transform.position;
                var currentPosition = context.Owner.transform.position;
                var direction = waitPosition - currentPosition;
                if (direction.magnitude < 0.5f)
                {
                    context.GoToState(new DistractionStateWaiting(_spawner));
                }
                else
                {
                    context.Owner.GetComponent<TwoDeeMovementComponent>().Direction.Value = direction.normalized;
                }
            };
        }
    }
}