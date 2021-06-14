﻿using System.Linq;
using SystemBase.StateMachineBase;
using Systems.DistractionManagement;
using Systems.Distractions.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(typeof(GoingToDistraction), typeof(GoingBackToIdle), typeof(WalkingOutOfLevel))]
    public class PickingInterest : BaseState<TouristBrainComponent>
    {
        private readonly DistractionFinder _distractionFinder;
        private float _distractionSearchRange;

        public PickingInterest()
        {
            _distractionFinder = new DistractionFinder();
        }

        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            _distractionSearchRange = context.Owner.DistractionSearchRange;
            context.Owner.UpdateAsObservable()
                .Subscribe(_ => PickInterestOrLeaveIt(context))
                .AddTo(this);
        }

        private void PickInterestOrLeaveIt(StateContext<TouristBrainComponent> context)
        {
            var stop = Random.value;
            if (stop < context.Owner.StoprInterestPropability)
            {
                context.GoToState(new GoingBackToIdle(Random.insideUnitCircle));
            }

            var nearestDistraction = _distractionFinder
                .FindNearestDistraction(context.Owner.transform, _distractionSearchRange);
            if (nearestDistraction)
            {
                context.GoToState(new GoingToDistraction(nearestDistraction));
            }
            else
            {
                _distractionSearchRange += context.Owner.DistractionSearchRangeIncrement;
            }
        }
    }

    internal class DistractionFinder
    {
        public DistractionOriginComponent FindNearestDistraction(Transform transform, float distance)
        {
            var nearest = Object.FindObjectsOfType<DistractionOriginComponent>()
                .Where(comp => !(comp.StateContext.CurrentState.Value is DistractionStateAborted))
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
