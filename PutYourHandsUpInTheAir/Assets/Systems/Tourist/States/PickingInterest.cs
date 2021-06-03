using System.Linq;
using SystemBase.StateMachineBase;
using Systems.DistractionControl;
using Systems.Distractions2;
using Systems.Movement;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(GoingToAttraction), typeof(GoingBackToIdle), typeof(WalkingOutOfLevel))]
    public class PickingInterest : BaseState<TouristBrainComponent>
    {
        private readonly TouristBrainComponent _tourist;
        private readonly DistractionFinder _distractionFinder;
        private float _distractionSearchRange;

        public PickingInterest(TouristBrainComponent tourist)
        {
            _tourist = tourist;
            _distractionSearchRange = _tourist.DistractionSearchRange;
            _distractionFinder = new DistractionFinder();
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            _tourist.UpdateAsObservable()
                .Subscribe(_ => PickInterestOrLeaveIt(context))
                .AddTo(this);
        }

        private void PickInterestOrLeaveIt(StateContext<TouristBrainComponent> context)
        {
            var stop = Random.value;
            if (stop < _tourist.StoprInterestPropability)
            {
                context.GoToState(new GoingBackToIdle(Random.insideUnitCircle, _tourist));
            }

            var nearestDistraction = _distractionFinder
                .FindNearestDistraction(_tourist.transform, _distractionSearchRange);
            if (nearestDistraction)
            {
                context.GoToState(new GoingToAttraction(nearestDistraction.InteractionPosition));
                _tourist.GetComponent<DistractableComponent>().DistractionType.Value =
                    nearestDistraction.DistractionType;
            }
            else
            {
                _distractionSearchRange += _tourist.DistractionSearchRangeIncrement;
            }
        }
    }

    internal class DistractionFinder
    {
        public DistractionOriginComponent FindNearestDistraction(Transform transform, float distance)
        {
            var nearest = Object.FindObjectsOfType<DistractionOriginComponent>()
                .OrderBy(component => (component.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (nearest != null 
                && (nearest.transform.position - transform.position).sqrMagnitude < distance * distance)
            {
                return nearest;
            }

            return null;
        }
    }
}
