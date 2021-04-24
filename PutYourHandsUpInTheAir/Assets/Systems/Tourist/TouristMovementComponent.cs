using System.Collections;
using System.Collections.Generic;
using SystemBase;
using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist
{
    public class TouristMovementComponent : GameComponent
    {
        public float speed = 1f;

        public StateContext<TouristMovementComponent> States { get; } =
            new StateContext<TouristMovementComponent>();
    }
}