using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.Distractions;
using UnityEngine;

namespace Systems.DistractionManagement
{
    public class DistractionPrefabs : GameComponent
    {
        [SerializeField]
        public List<DistractionInfo> Distractions = new List<DistractionInfo>();

        public GameObject GetPrefab(DistractionType type)
        {
            return Distractions.First(info => info.Type == type).Prefab;
        }
    }
    
    [Serializable]
    public struct DistractionInfo
    {
        public DistractionType Type;
        public GameObject Prefab;
    }
}