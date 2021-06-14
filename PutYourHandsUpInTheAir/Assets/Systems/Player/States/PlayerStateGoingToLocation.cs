using SystemBase.StateMachineBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateIdle), typeof(PlayerStateGoingToDistraction))]
    public class PlayerStateGoingToLocation : BaseState<PlayerComponent>
    {
        private Vector3 _goToPosition;
        private readonly MouseClickTester _clickTester;

        public PlayerStateGoingToLocation(Vector3 hitPoint)
        {
            _goToPosition = hitPoint;
            _clickTester = new MouseClickTester();
        }

        public override void Enter(StateContext<PlayerComponent> context)
        {
            context.Owner.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (PlayerReachedDistraction(context)) return;
                    if (UserClickedSomething(context)) return;
                    UpdatePlayerDirection(context.Owner);
                })
                .AddTo(this);
        }

        private bool UserClickedSomething(StateContext<PlayerComponent> context)
        {
            if (!_clickTester.IsLeftMouseClicked()) return false;

            var clickedObject = _clickTester.CheckMouseClick(out var hit);
            switch (clickedObject)
            {
                case ClickedObject.Ground:
                    _goToPosition = hit.point;
                    return true;

                case ClickedObject.Distraction:
                    context.GoToState(new PlayerStateGoingToDistraction(hit.transform));
                    return true;

                case ClickedObject.Nothing:
                    // Goto Edge of PlayField
                    return false;

                default: return false;
            }
        }

        private bool PlayerReachedDistraction(StateContext<PlayerComponent> context)
        {
            var distanceVec = _goToPosition - context.Owner.transform.position;
            if (distanceVec.sqrMagnitude > 0.1) return false;

            context.GoToState(new PlayerStateIdle());
            return true;
        }

        private void UpdatePlayerDirection(Component player)
        {
            var distanceVec = _goToPosition - player.transform.position;
            player.GetComponent<TwoDeeMovementComponent>().Direction.Value = distanceVec.normalized;
        }
    }
}