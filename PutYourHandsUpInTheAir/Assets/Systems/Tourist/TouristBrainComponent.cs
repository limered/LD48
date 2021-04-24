using System.Collections;
using System.Collections.Generic;
using SystemBase;
using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.Tourist
{
    [RequireComponent(typeof(TouristBrainComponent))]
    public class TouristBrainComponent : GameComponent
    {
        public StateContext<TouristBrainComponent> States { get; } =
            new StateContext<TouristBrainComponent>();
    }
}