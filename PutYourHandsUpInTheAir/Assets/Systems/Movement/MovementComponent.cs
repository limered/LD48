using System;
using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementComponent : GameComponent
    {
        public float Speed;
        public float Friction;
        public float MaxSpeed;
        public Collider Collider;

        public Vector2ReactiveProperty Direction = new Vector2ReactiveProperty(Vector2.zero);
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

    }
}