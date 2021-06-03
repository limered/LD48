using System.Collections.Generic;
using System.Linq;
using Systems.Player.TouristInteraction;
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
                .Where(brain => !brain.GetComponent<IsNearPlayerComponent>())
                .Where(brain => brain.StateContext.CurrentState.Value is Idle)
                .ToList()
                .Randomize()
                .ForEach(t => queuedRandoms.Enqueue(t));

            return queuedRandoms;
        }
    }
}
