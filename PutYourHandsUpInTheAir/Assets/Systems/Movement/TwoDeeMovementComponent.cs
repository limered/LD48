using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class TwoDeeMovementComponent : GameComponent
    {
        public float Speed;
        public float Friction;

        
        public float FrictionPercent;

        public float VelocityCutoff;

        public Vector2ReactiveProperty Direction = new Vector2ReactiveProperty(Vector2.zero);

        public float CurrentVelocity;
        public BoolReactiveProperty IsMoving = new BoolReactiveProperty();
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        // not used
        public Collider Collider { get; set; }

        
        
    }
}