using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Distractions.States;
using Systems.Movement;
using UniRx;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(GoingBackToIdle), typeof(Dead), typeof(WalkingOutOfLevel))]
    public class Interacting : BaseState<TouristBrainComponent>
    {
        private readonly DistractionOriginComponent _distraction;

        public Interacting(DistractionOriginComponent distraction)
        {
            _distraction = distraction;
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            context.Owner.GetComponent<TwoDeeMovementComponent>().SlowStop();
            _distraction.StateContext.CurrentState.Where(state => state is DistractionStateAborted)
                .Subscribe(_ => context.GoToState(new GoingBackToIdle(Random.insideUnitCircle)))
                .AddTo(this);
        }
    }
}