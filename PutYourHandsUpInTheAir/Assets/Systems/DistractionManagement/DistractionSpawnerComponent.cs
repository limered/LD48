using SystemBase;
using Systems.Distractions;
using UnityEngine;

namespace Systems.DistractionManagement
{
    public class DistractionSpawnerComponent : GameComponent
    {
        public DistractionType DistractionType;
        public GameObject WaitArea;
        public GameObject DespawnPosition;

        public float TimeBetweenSpawnsInSec;
        public float WaitTimebeforeRespawn = 2.0f;
        public DistractionOriginComponent CurrentDistraction;

        public Vector3 WaitPosition;
    }
}