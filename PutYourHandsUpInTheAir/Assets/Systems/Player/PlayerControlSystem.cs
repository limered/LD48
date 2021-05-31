using SystemBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Player
{
    [GameSystem(typeof(TwoDeeMovementSystem))]
    public class PlayerControlSystem : GameSystem<PlayerComponent>
    {
        private readonly int floorLayer = LayerMask.NameToLayer("Floor");
        private readonly int distractionLayer = LayerMask.NameToLayer("Distraction");

        public override void Register(PlayerComponent component)
        {
            component.UpdateAsObservable()
                .Select(_ => (movement: component.GetComponent<TwoDeeMovementComponent>(), player: component))
                .Subscribe(ControlPlayer)
                .AddTo(component);
        }

        private void ControlPlayer((TwoDeeMovementComponent movement, PlayerComponent player) playerMovement)
        {
            var (movement, player) = playerMovement;
            SetPlayerDirection(movement, player);

            SetPlayerTargetOnClick(player);
        }

        private void SetPlayerTargetOnClick(PlayerComponent player)
        {
            if (!Input.GetMouseButtonDown((int) MouseButton.LeftMouse)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _, Mathf.Infinity, 1 << distractionLayer)) return;
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << floorLayer)) return;

            player.TargetedDistraction.Value = null;
            player.TargetVector = new Vector3(hit.point.x, hit.point.y);
        }

        private static void SetPlayerDirection(TwoDeeMovementComponent movement, PlayerComponent player)
        {
            movement.Direction.Value = (player.TargetVector - movement.transform.position).normalized;
        }
    }
}