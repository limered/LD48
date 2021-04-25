using SystemBase;
using SystemBase.StateMachineBase;
using UnityEngine;

namespace Systems.MacheteMan
{
    public class MacheteManComponent : GameComponent
    {
        [HideInInspector]
        public Animator animator;
        public Vector2 TargetPosition { get; set; }
        public StateContext<MacheteManComponent> State { get; } = new StateContext<MacheteManComponent>();
    }
}