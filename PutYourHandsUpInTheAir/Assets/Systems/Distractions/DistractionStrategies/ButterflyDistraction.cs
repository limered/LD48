using System;
using Systems.Distractions.Messages;
using Systems.Distractions.States;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UnityEngine;

namespace Systems.Distractions.DistractionStrategies
{
    public class ButterflyDistraction : IDistraction
    {
        private float _maxDistractionDuration = 3.0f;
        private float _lastDistractionProgressTime = 3.0f;
        private bool _cancelledByPlayer;
        private IDisposable _abortedDisposable;

        public void Init(DistractableComponent distractable)
        {
            _maxDistractionDuration = distractable.Origin.DistractionInteractionDuration;
            _lastDistractionProgressTime = _maxDistractionDuration;

            _abortedDisposable = distractable.Origin.StateContext.CurrentState
                .Where(state => state is DistractionStateAborted)
                .Subscribe(_ => _cancelledByPlayer = true);
        }

        public DistractionOutcome Update(DistractableComponent distractable)
        {
            if (_cancelledByPlayer)
            {
                _abortedDisposable.Dispose();
                return DistractionOutcome.Alive;
            }

            var touristBrain = distractable.GetComponent<TouristBrainComponent>();
            if (touristBrain.StateContext.CurrentState.Value is Interacting)
            {
                _lastDistractionProgressTime -= Time.fixedDeltaTime;
                if (_lastDistractionProgressTime <= 0)
                {
                    MessageBroker.Default.Publish(new AbortDistractionAction(distractable.Origin));
                    return DistractionOutcome.Alive;
                }
            }

            distractable.DistractionProgress.Value = 1 - _lastDistractionProgressTime / _maxDistractionDuration;
            return DistractionOutcome.Waiting;
        }
    }
}
