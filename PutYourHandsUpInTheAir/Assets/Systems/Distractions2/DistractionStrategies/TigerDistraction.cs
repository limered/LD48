using Systems.Tourist;
using Systems.Tourist.States;
using UnityEngine;

namespace Systems.Distractions2.DistractionStrategies
{
    public class TigerDistraction : IDistraction
    {
        private const float MaxDistractionDuration = 3.0f;
        private float LastDistractionProgressTime = MaxDistractionDuration;

        public void Init()
        {
        }

        public DistractionOutcome Update(DistractableComponent distractable)
        {
            var touristBrain = distractable.GetComponent<TouristBrainComponent>();

            if (touristBrain.StateContext.CurrentState.Value is Interacting)
            {
                LastDistractionProgressTime -= Time.fixedDeltaTime;
                if (LastDistractionProgressTime <= 0)
                {
                    return DistractionOutcome.Dead;
                }
            }

            distractable.DistractionProgress.Value = 1 - LastDistractionProgressTime / MaxDistractionDuration;
            return DistractionOutcome.Waiting;
        }
    }
}
