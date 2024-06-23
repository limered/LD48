using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Player.States
{
    [NextValidStates(typeof(PlayerStateGoingToLocation), typeof(PlayerStateHittingDistraction))]
    public class PlayerStateGoingToDistraction : BaseState<PlayerComponent>
    {
        private DistractionOriginComponent _distraction;
        private readonly MouseClickTester _clickTester;

        public PlayerStateGoingToDistraction(Component distractionLocation)
        {
            _distraction = distractionLocation.GetComponent<DistractionOriginComponent>();
            _clickTester = new MouseClickTester();
        }

        public override void Enter(StateContext<PlayerComponent> context)
        {
            context.Owner.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (PlayerReachedGoal(context)) return;
                    if (UserClickedSomething(context)) return;
                    UpdatePlayerDirection(context.Owner);

                })
                .AddTo(this);
        }

        private bool PlayerReachedGoal(StateContext<PlayerComponent> context)
        {
            var distanceVec = _distraction.transform.position - context.Owner.transform.position;
            if (distanceVec.magnitude > context.Owner.DistractionHittingDistance) return false;

            context.GoToState(new PlayerStateHittingDistraction(_distraction));
            return true;
        }

        private bool UserClickedSomething(StateContext<PlayerComponent> context)
        {
            if (!_clickTester.IsLeftMouseClicked()) return false;

            var clickedObject = _clickTester.CheckMouseClick(out var hit);
            switch (clickedObject)
            {
                case ClickedObject.Distraction:
                    _distraction = hit.transform.GetComponent<DistractionOriginComponent>();
                    return true;

                case ClickedObject.Ground:
                    context.GoToState(new PlayerStateGoingToLocation(hit.point));
                    return true;

                case ClickedObject.Nothing:
                    // Goto Edge of PlayField
                    return false;

                default: return false;
            }
        }

        private void UpdatePlayerDirection(Component player)
        {
            var distanceVec = _distraction.transform.position - player.transform.position;
            player.GetComponent<TwoDeeMovementComponent>().Direction.Value = distanceVec.normalized;
        }
    }
}
