using Systems.Tourist;
using Systems.Tourist.States;
using UnityEngine;

namespace Systems.Distractions2.DistractionStrategies
{
    public class ButterflyDistraction : IDistraction
    {
        private const float MaxDistractionDuration = 3.0f;
        private float LastDistractionProgressTime = MaxDistractionDuration;

        public void Init()
        {
        }

        public DistractionOutcome Update(DistractableComponent distractable)
        {
            var touristBrain = distractable.GetComponent<TouristBrainComponent>();

            if (touristBrain.States.CurrentState.Value is Interacting)
            {
                LastDistractionProgressTime -= Time.fixedDeltaTime;
                if (LastDistractionProgressTime <= 0)
                {
                    touristBrain.States.GoToState(new GoingBackToIdle(Vector2.zero));
                    return DistractionOutcome.Alive;
                }
            }

            distractable.DistractionProgress.Value = 1 - LastDistractionProgressTime / MaxDistractionDuration;
            return DistractionOutcome.Waiting;
        }
    }
}
