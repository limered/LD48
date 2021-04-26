using SystemBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Unity;
using Object = UnityEngine.Object;

namespace Systems.Player
{
    [GameSystem(typeof(MovementSystem))]
    public class PlayerSystem : GameSystem<PlayerComponent, MovementComponent, PlayerSpawnerComponent>
    {
        public override void Register(PlayerComponent component)
        {
            component.GetComponent<MovementComponent>().Direction
                .Subscribe(dir => component.IsMoving.Value = dir.magnitude > 0);
        }

        public override void Register(MovementComponent component)
        {
            component.UpdateAsObservable()
                .Select(_ => component)
                .Where(movementComponent => movementComponent.GetComponent<PlayerComponent>())
                .Subscribe(ControlPlayer)
                .AddTo(component);
        }

        private static void ControlPlayer(MovementComponent comp)
        {
            var floorLayer = LayerMask.NameToLayer("Floor");
            var player = comp.GetComponent<PlayerComponent>();

            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                Debug.Log("click");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << floorLayer)) return;

                Debug.Log(hit.point.x);
                player.TargetedTourist = null;
                player.TargetVector = new Vector3(hit.point.x, hit.point.y);
            }

            var targetVec = player.TargetedTourist != null
                ? player.TargetedTourist.transform.position 
                : player.TargetVector;

            comp.Direction.Value = (targetVec - comp.transform.position).normalized;
        }

        public override void Register(PlayerSpawnerComponent component)
        {
            MessageBroker.Default.Receive<ActPlayerRespawn>()
                .Subscribe(respawn =>
                    Object.Instantiate(component.PlayerPrefab, component.transform.position, Quaternion.identity));
        }
    }
}