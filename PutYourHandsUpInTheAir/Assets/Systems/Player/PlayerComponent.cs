using SystemBase;
using Systems.DistractionControl;
using Systems.Distractions2;
using Systems.Movement;
using Systems.Tourist;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    [RequireComponent(typeof(TwoDeeMovementComponent))]
    public class PlayerComponent : GameComponent
    {
        public ReactiveProperty<DistractionOriginComponent> TargetedDistraction = new ReactiveProperty<DistractionOriginComponent>();
        public Vector3 TargetVector;
    }
}