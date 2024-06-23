using System;
using SystemBase.StateMachineBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateGoingToLocation), typeof(PlayerStateGoingToDistraction))]
    public class PlayerStateIdle : BaseState<PlayerComponent>
    {
        private readonly MouseClickTester _clickTester;

        public PlayerStateIdle()
        {
            _clickTester = new MouseClickTester();
        }

        public override void Enter(StateContext<PlayerComponent> context)
        {
            context.Owner.GetComponent<TwoDeeMovementComponent>().SlowStop();

            context.Owner.UpdateAsObservable()
                .Subscribe(ReactOnClick(context))
                .AddTo(this);
        }

        private Action<Unit> ReactOnClick(StateContext<PlayerComponent> context)
        {
            return _ =>
            {
                if (!_clickTester.IsLeftMouseClicked()) return;

                var clicked = _clickTester.CheckMouseClick(out var hit);

                switch (clicked)
                {
                    case ClickedObject.Ground:
                        context.GoToState(new PlayerStateGoingToLocation(hit.point));
                        break;
                    case ClickedObject.Distraction:
                        context.GoToState(new PlayerStateGoingToDistraction(hit.transform));
                        break;
                    case ClickedObject.Nothing:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }
    }
}