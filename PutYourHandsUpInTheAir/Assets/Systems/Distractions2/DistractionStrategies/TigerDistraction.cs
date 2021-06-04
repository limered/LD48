using SystemBase.StateMachineBase;
using Systems.Tourist;
using Systems.Tourist.States;
using UnityEngine;

namespace Systems.Distractions2.DistractionStrategies
{
    public class TigerDistraction : IDistraction
    {
        private float MaxDistractionDuration = 3.0f;
        private float LastDistractionProgressTime = 3.0f;

        public void Init(DistractableComponent distractable)
        {
            MaxDistractionDuration = distractable.Origin.DistractionInteractionDuration;
            LastDistractionProgressTime = MaxDistractionDuration;
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
