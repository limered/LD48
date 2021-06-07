using SystemBase;
using Systems.DistractionManagement;
using Systems.Distractions;
using Systems.Movement;
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