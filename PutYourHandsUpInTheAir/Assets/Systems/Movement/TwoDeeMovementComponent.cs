﻿using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class TwoDeeMovementComponent : GameComponent
    {
        public float Speed;

        [Range(0.0f, 1.0f)]
        public float Drag;
        public float MaxVelocity;

        public Vector2ReactiveProperty Direction = new Vector2ReactiveProperty(Vector2.zero);

        public float CurrentVelocity;
        public BodyData BodyData;
        public BoolReactiveProperty IsMoving = new BoolReactiveProperty();
        

        // not used
        public Collider Collider { get; set; }
        
    }

    public struct BodyData
    {
        public Vector2 Velocity;
        public Vector2 Acceleration;
    }
}