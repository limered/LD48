using SystemBase;
using Systems.Distractions2;
using UnityEngine;

namespace Systems.DistractionControl
{
    public class DistractionOriginComponent : GameComponent
    {
        public DistractionType DistractionType;
        public float DistractionInteractionDuration;
        public Color DistractionProgressColor;
        public Transform InteractionPosition;
        public SphereCollider InteractionCollider;
        public bool FireOnce = false;

        public bool HasFired { get; set; }
    }
}