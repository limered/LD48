using System.Collections.Generic;
using System.Linq;
using Systems.DistractionControl;
using Systems.Tourist;
using Systems.Tourist.States;
using UnityEngine;
using Utils.DotNet;

namespace Systems.Distractions
{
    public class RandomTouristFinder
    {
        public TouristBrainComponent FindRandomTouristWithoutDistraction()
        {
            List<TouristBrainComponent> touristBrains = GameObject.FindGameObjectsWithTag("tourist")
                .Select(t => t.GetComponent<TouristBrainComponent>())
                .Where(brain => brain.States.CurrentState.Value is Idle)
                .ToList();

            return GerRandomTouristWithoutDistraction(touristBrains);
        }

        private TouristBrainComponent GerRandomTouristWithoutDistraction(List<TouristBrainComponent> touristBrains)
        {
            return touristBrains
                .Randomize()
                .FirstOrDefault(touristBrain => touristBrain.GetComponent<DistractionComponent>() == null);
        }
    }
}
