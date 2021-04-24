using SystemBase;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Unity;
using Object = UnityEngine.Object;

namespace Systems.Player
{
    [GameSystem(typeof(MovementSystem))]
    public class PlayerSystem : GameSystem<PlayerComponent, MovementComponent, PlayerSpawnerComponent>
    {
        private GameObject _currentPlayer;

        public override void Register(PlayerComponent component)
        {
            if (_currentPlayer)
            {
                Object.Destroy(component.gameObject);
            }
            _currentPlayer = component.gameObject;

            component.GetComponent<MovementComponent>().Direction
                .Subscribe(dir => component.IsMoving.Value = dir.magnitude > 0);

            component.IsMoving.Subscribe(obj => StartMovingState(component)).AddTo(component);

        }

        private void StartMovingState(PlayerComponent component)
        {
            //component.GetComponentInChildren<Animator>().SetFloat("Speed", component.IsMoving.Value ? 7f : 0f);
        }

        public override void Register(MovementComponent component)
        {
            component.FixedUpdateAsObservable()
                .Select(_ => component)
                .Where(movementComponent => movementComponent.GetComponent<PlayerComponent>())
                .Subscribe(ControlPlayer)
                .AddTo(component);
        }

        private static void ControlPlayer(MovementComponent comp)
        {
            var movementDirection = Vector2.zero;
            if (KeyCode.W.IsPressed())
            {
                movementDirection.y += 1;
            }

            if (KeyCode.S.IsPressed())
            {
                movementDirection.y += -1;
            }

            if (KeyCode.A.IsPressed())
            {
                movementDirection.x += -1;
            }

            if (KeyCode.D.IsPressed())
            {
                movementDirection.x += 1;
            }

            comp.Direction.Value = movementDirection;
        }

        public override void Register(PlayerSpawnerComponent component)
        {
            MessageBroker.Default.Receive<ActPlayerRespawn>()
                .Subscribe(respawn =>
                    Object.Instantiate(component.PlayerPrefab, component.transform.position, Quaternion.identity));
        }
    }
}