using System.Collections.Generic;
using System.Linq;
using Systems.Distractions;
using Systems.Tourist;
using Systems.Tourist.States;
using UnityEngine;
using Utils.DotNet;

namespace Systems.DistractionControl
{
    public class RandomTouristFinder
    {
        public Queue<TouristBrainComponent> FindTouristsWithoutDistraction()
        {
            var queuedRandoms = new Queue<TouristBrainComponent>();

            GameObject.FindGameObjectsWithTag("tourist")
                .Select(t => t.GetComponent<TouristBrainComponent>())
                .Where(brain => brain.States.CurrentState.Value is Idle)
                .ToList()
                .Randomize()
                .ForEach(t => queuedRandoms.Enqueue(t));


            return queuedRandoms;
        }

        private TouristBrainComponent GetRandomTouristWithoutDistraction(List<TouristBrainComponent> touristBrains)
        {
            return touristBrains
                .Randomize()
                .FirstOrDefault(touristBrain => touristBrain.GetComponent<DistractedTouristComponent>() == null);
        }
    }
}
