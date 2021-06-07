using System.Collections.Generic;
using SystemBase;
using Systems.Distractions;
using UnityEngine;

namespace Systems.DistractionManagement
{
    public class DistractionPrefabs : GameComponent
    { 
        public Dictionary<DistractionType, GameObject> Distractions = new Dictionary<DistractionType, GameObject>();

        public GameObject GetPrefab(DistractionType type)
        {
            return Distractions[type];
        }
    }
}