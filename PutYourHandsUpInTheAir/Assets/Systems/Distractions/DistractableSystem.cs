using System;
using SystemBase;
using Systems.Distractions.DistractionStrategies;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using Random = UnityEngine.Random;

namespace Systems.Distractions
{
    [GameSystem]
    public class DistractableSystem : GameSystem<DistractableComponent>
    {
        public override void Register(DistractableComponent component)
        {
            component.DistractionType
                .Where(dType => dType != DistractionType.None)
                .Subscribe(StartTouristDistraction(component))
                .AddTo(component);
        }

        private Action<DistractionType> StartTouristDistraction(DistractableComponent component)
        {
            return type =>
            {
                component.DistractionProgress.Value = 0;
                component.ActiveDistractionStrategy = CreateDistraction(type);
                component.ActiveDistractionStrategy.Init(component);
                component.DistractionUpdateObservable = SystemFixedUpdate(component)
                    .Subscribe(UpdateDistraction());
            };
        }

        private Action<DistractableComponent> UpdateDistraction()
        {
            return distractable =>
            {
                var tourist = distractable.GetComponent<TouristBrainComponent>();
                if (!tourist) return;
                var outcome = distractable.ActiveDistractionStrategy.Update(distractable);
                switch (outcome)
                {
                    case DistractionOutcome.Waiting:
                        break;
                    case DistractionOutcome.Alive:
                        tourist.StateContext.GoToState(new GoingBackToIdle(Random.insideUnitCircle));
                        StopTouristDistraction(distractable);
                        break;
                    case DistractionOutcome.Dead:
                        tourist.StateContext.GoToState(new Dead(distractable.DistractionType.Value));
                        StopTouristDistraction(distractable);
                        break;
                    case DistractionOutcome.Debuffed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        private static void StopTouristDistraction(DistractableComponent distractable)
        {
            distractable.DistractionType.Value = DistractionType.None;
            distractable.DistractionProgress.Value = 1;
            distractable.ActiveDistractionStrategy = new NoneDistraction();
            distractable.DistractionUpdateObservable.Dispose();
            distractable.DistractionUpdateObservable = null;
            distractable.Origin = null;
        }

        private IDistraction CreateDistraction(DistractionType type)
        {
            return type switch
            {
                DistractionType.None => (IDistraction) new NoneDistraction(),
                DistractionType.Tiger => new TigerDistraction(),
                DistractionType.Butterfly => new ButterflyDistraction(),
                DistractionType.Spider => new SpiderDistraction(),
                DistractionType.Swamp => throw new ArgumentOutOfRangeException(nameof(type), type, "Not implemented yet"),
                DistractionType.Camera => throw new ArgumentOutOfRangeException(nameof(type), type, "Not implemented"),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
