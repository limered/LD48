using SystemBase;
using Systems.Distractions;
using UnityEngine;

namespace Systems.DistractionManagement
{
    public class DistractionSpawnerComponent : GameComponent
    {
        public DistractionType DistractionType;
        public GameObject WaitPosition;
        public GameObject HidePosition;

        public float TimeBetweenSpawnsInSec;
    }
}