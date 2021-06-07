using Systems.Tourist;
using Systems.Tourist.States;
using UnityEngine;

namespace Systems.Distractions.DistractionStrategies
{
    public class ButterflyDistraction : IDistraction
    {
        private const float MaxDistractionDuration = 3.0f;
        private float LastDistractionProgressTime = MaxDistractionDuration;

        public void Init(DistractableComponent distractable)
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
                    touristBrain.StateContext.GoToState(new GoingBackToIdle(Random.insideUnitCircle));
                    return DistractionOutcome.Alive;
                }
            }

            distractable.DistractionProgress.Value = 1 - LastDistractionProgressTime / MaxDistractionDuration;
            return DistractionOutcome.Waiting;
        }
    }
}
