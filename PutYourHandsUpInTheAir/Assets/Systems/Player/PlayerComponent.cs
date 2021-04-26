using SystemBase;
using Systems.Tourist;
using UniRx;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerComponent : GameComponent
    {
        public BoolReactiveProperty IsMoving;
        public TouristBrainComponent TargetedTourist;
        public Vector3 TargetVector;

        public bool TouristOnlyMode;
        public bool TouristClicked { get; set; }
        public TouristBrainComponent LastTargetetTourist { get; set; }
    }
}