﻿using System;
using Systems.Tourist.States;
using UnityEngine;

namespace Systems.Tourist
{
    [Serializable]
    public class TouristDump
    {
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public int HeadPartIndex { get; set; }
        public int BodyPartIndex { get; set; }
        public float SocialDistancingRadius { get; set; }
        
        //TODO: also save speed values?

        public TouristDump(){}
        
        public TouristDump(TouristBrainComponent brain)
        {
            Name = brain.touristName.Value;
            IsAlive = !(brain.StateContext.CurrentState.Value is Dead);
            HeadPartIndex = brain.headPartIndex.Value;
            BodyPartIndex = brain.bodyPartIndex.Value;
            SocialDistancingRadius = brain.socialDistanceCollider != null ? brain.socialDistanceCollider.radius : 0.5f;
        }

        public void Apply(TouristBrainComponent toBrain)
        {
            if (IsAlive)
            {
                toBrain.touristName.Value = Name;
                toBrain.headPartIndex.Value = HeadPartIndex;
                toBrain.bodyPartIndex.Value = BodyPartIndex;
                toBrain.socialDistanceCollider.radius = SocialDistancingRadius;
            }
            else
            {
                Debug.Log($"cannot revive a {nameof(Dead)} Tourist ({Name})");
            }
        }
    }
}