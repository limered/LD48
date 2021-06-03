using System;
using SystemBase;
using Systems.Distractions2.DistractionStrategies;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;

namespace Systems.Distractions2
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
                component.ActiveDistraction = CreateDistraction(type);
                component.ActiveDistraction.Init();
                component.DistractionUpdateObservable = SystemFixedUpdate(component)
                    .Subscribe(UpdateDistraction());
            };
        }

        private Action<DistractableComponent> UpdateDistraction()
        {
            return distractable =>
            {
                var outcome = distractable.ActiveDistraction.Update(distractable);
                switch (outcome)
                {
                    case DistractionOutcome.Waiting:
                        break;
                    case DistractionOutcome.Alive:
                        StopTouristDistraction(distractable);
                        break;
                    case DistractionOutcome.Dead:
                        var tourist = distractable.GetComponent<TouristBrainComponent>();
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
            distractable.ActiveDistraction = new NoneDistraction();
            distractable.DistractionUpdateObservable.Dispose();
            distractable.DistractionUpdateObservable = null;
        }

        private IDistraction CreateDistraction(DistractionType type)
        {
            return type switch
            {
                DistractionType.None => (IDistraction) new NoneDistraction(),
                DistractionType.Tiger => new TigerDistraction(),
                DistractionType.Butterfly => new ButterflyDistraction(),
                DistractionType.Spider => new SpiderDistraction(),
                DistractionType.Money => new MoneyDistraction(),
                DistractionType.Bus => new BusDistraction(),
                DistractionType.Swamp => throw new ArgumentOutOfRangeException(nameof(type), type, "Not implemented yet"),
                DistractionType.Camera => throw new ArgumentOutOfRangeException(nameof(type), type, "Not implemented"),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
