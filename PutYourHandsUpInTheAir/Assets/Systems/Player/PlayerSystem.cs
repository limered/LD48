using SystemBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Player
{
    [GameSystem(typeof(TwoDeeMovementSystem))]
    public class PlayerSystem : GameSystem<PlayerComponent>
    {
        public override void Register(PlayerComponent component)
        {
            component.UpdateAsObservable()
                .Select(_ => component.GetComponent<TwoDeeMovementComponent>())
                .Subscribe(ControlPlayer)
                .AddTo(component);
        }

        private static void ControlPlayer(TwoDeeMovementComponent comp)
        {
            var floorLayer = LayerMask.NameToLayer("Floor");
            var touristLayer = LayerMask.NameToLayer("Tourist");
            var player = comp.GetComponent<PlayerComponent>();

            var targetVec = player.TargetedTourist.Value != null
                ? player.TargetedTourist.Value.transform.position 
                : player.TargetVector;

            comp.Direction.Value = (targetVec - comp.transform.position).normalized;

            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) 
                && !player.TouristOnlyMode)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var _tHit, Mathf.Infinity, 1 << touristLayer)) return;
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << floorLayer)) return;

                player.LastTargetedTourist.Value = player.TargetedTourist.Value;
                player.TargetedTourist.Value = null;
                player.TargetVector = new Vector3(hit.point.x, hit.point.y);
            }
        }
    }
}