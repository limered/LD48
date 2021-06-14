using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateGoingToLocation), typeof(PlayerStateGoingToDistraction), typeof(PlayerStateIdle))]
    public class PlayerStateHittingDistraction : BaseState<PlayerComponent>
    {
        private DistractionOriginComponent _distraction;
        private float TimeLeftToHit;
        private readonly MouseClickTester _clickTester;

        public PlayerStateHittingDistraction(DistractionOriginComponent distraction)
        {
            _distraction = distraction;
            _clickTester = new MouseClickTester();
        }

        public override void Enter(StateContext<PlayerComponent> context)
        {
            context.Owner.GetComponent<TwoDeeMovementComponent>().SlowStop();
            TimeLeftToHit = context.Owner.DistractionHittingDuration;
            context.Owner.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    CountDown(context);
                    UserClickedSomething(context);
                })
                .AddTo(this);
        }

        private void CountDown(StateContext<PlayerComponent> context)
        {
            TimeLeftToHit -= Time.deltaTime;
            if (TimeLeftToHit < 0.0)
            {
                context.GoToState(new PlayerStateIdle());
            }
            else if (TimeLeftToHit < 0.1)
            {
                // Hit Message
            }
        }

        private void UserClickedSomething(StateContext<PlayerComponent> context)
        {
            if (!_clickTester.IsLeftMouseClicked()) return;

            var clickedObject = _clickTester.CheckMouseClick(out var hit);
            switch (clickedObject)
            {
                case ClickedObject.Ground:
                    context.GoToState(new PlayerStateGoingToLocation(hit.point));
                    return;

                case ClickedObject.Distraction:
                    context.GoToState(new PlayerStateGoingToDistraction(hit.transform));
                    return;

                case ClickedObject.Nothing:
                    // Goto Edge of PlayField
                    return;

                default:
                    return;
            }
        }
    }
}