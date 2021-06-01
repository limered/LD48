using System;
using SystemBase;
using Systems.Distractions;
using Systems.Distractions2.DistractionStrategies;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;

namespace Systems.Distractions2
{
    [GameSystem]
    public class DistractionSystem : GameSystem<DistractableComponent>
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
                        tourist.States.GoToState(new Dead(new TigerDistractionTouristComponent()));
                        StopTouristDistraction(distractable);
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

        private IDIstraction CreateDistraction(DistractionType type)
        {
            switch (type)
            {
                case DistractionType.None:
                    break;
                case DistractionType.Tiger:
                    return new TigerDistraction();
                case DistractionType.Butterfly:
                    return new ButterflyDistraction();
                case DistractionType.Spider:
                    return new SpiderDistraction();
                case DistractionType.Money:
                    return new MoneyDistraction();
                case DistractionType.Bus:
                    return new BusDistraction();
                case DistractionType.Swamp:
                case DistractionType.Camera:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return new NoneDistraction();
        }
    }
}
