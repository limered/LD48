using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Distractions.Messages;
using Systems.Movement;
using UniRx;

namespace Systems.Distractions.States
{
    [NextValidStates(typeof(DistractionStateAborted))]
    public class DistractionStateWaiting : BaseState<DistractionOriginComponent>
    {
        private readonly DistractionSpawnerComponent _spawner;

        public DistractionStateWaiting(DistractionSpawnerComponent spawner)
        {
            _spawner = spawner;
        }

        public override void Enter(StateContext<DistractionOriginComponent> context)
        {
            context.Owner.GetComponent<TwoDeeMovementComponent>().Stop();

            MessageBroker.Default.Receive<AbortDistractionAction>()
                .Where(msg => msg.Distraction == context.Owner)
                .Subscribe(_ => context.GoToState(new DistractionStateAborted(_spawner)))
                .AddTo(this);
        }
    }
}