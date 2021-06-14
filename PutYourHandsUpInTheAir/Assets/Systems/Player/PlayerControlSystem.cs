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
            if (!IsLeftMouseClicked()) return;
            if (!HasClickedOnGround(out var hit)) return;

            player.TargetedDistraction.Value = null;
            player.TargetVector = new Vector3(hit.point.x, hit.point.y);
        }

        private bool HasClickedOnGround(out RaycastHit hit)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << distractionLayer)) return false;
            return Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << floorLayer);
        }

        private static bool IsLeftMouseClicked()
        {
            return Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
        }

        private static void SetPlayerDirection(TwoDeeMovementComponent movement, PlayerComponent player)
        {
            movement.Direction.Value = player.TargetedDistraction.Value 
                ? (player.TargetedDistraction.Value.transform.position - movement.transform.position).normalized 
                : (player.TargetVector - movement.transform.position).normalized;
        }
    }
}