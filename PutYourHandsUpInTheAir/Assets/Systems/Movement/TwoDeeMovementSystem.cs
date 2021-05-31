using SystemBase;
using GameState.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;

namespace Systems.Movement
{
    [GameSystem]
    public class TwoDeeMovementSystem : GameSystem<TwoDeeMovementComponent>
    {
        public override void Register(TwoDeeMovementComponent component)
        {
            component.FixedUpdateAsObservable()
                .Select(_ => component)
                .Subscribe(CalculateMovement)
                .AddTo(component);

            ResetRigidbody(component);

            component.Direction
                .Subscribe(dir => component.IsMoving.Value = dir.magnitude > 0);
        }

        private static void ResetRigidbody(Component comp)
        {
            var body = comp.GetComponent<Rigidbody>();
            body.drag = 0;
            body.angularDrag = 0;
            body.isKinematic = false;
            body.useGravity = false;
            body.freezeRotation = true;
        }

        private static void FixCollider(TwoDeeMovementComponent component)
        {
            component.Collider.transform.localPosition = Vector3.zero;
        }

        private static void ApplyAnimationToObject(TwoDeeMovementComponent component)
        {
            var positionChange = component.BodyData.Velocity * Time.fixedDeltaTime;
            component.transform.position = new Vector3(
                component.transform.position.x + positionChange.x,
                component.transform.position.y + positionChange.y,
                0.1f);

            component.CurrentVelocity = component.BodyData.Velocity.magnitude;
        }

        private static void Animate(TwoDeeMovementComponent component)
        {
            var futureVel = component.BodyData.Velocity + component.BodyData.Acceleration * Time.fixedDeltaTime;
            var speed = futureVel.sqrMagnitude;
            if (speed < component.MaxVelocity * component.MaxVelocity)
            {
                component.BodyData.Velocity = futureVel;
            }
            else
            {
                component.BodyData.Velocity = component.BodyData.Velocity.normalized * component.MaxVelocity;
            }
        }

        private static void ApplyFriction(TwoDeeMovementComponent component)
        {
            component.BodyData.Velocity *= 1 - component.Drag;
        }

        private static void ApplyDirection(TwoDeeMovementComponent component)
        {
            component.BodyData.Acceleration = component.Direction.Value * component.Speed;
        }

        private void CalculateMovement(TwoDeeMovementComponent component)
        {
            if (!(IoC.Game.GameStateContext.CurrentState.Value is Running)) return;

            StopRigidbodyMovement(component);
            ApplyDirection(component);
            ApplyFriction(component);
            Animate(component);
            ApplyAnimationToObject(component);
            if (component.Collider) FixCollider(component);
        }

        private static void StopRigidbodyMovement(TwoDeeMovementComponent component)
        {
            var body = component.GetComponent<Rigidbody>();
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
        }
    }
}