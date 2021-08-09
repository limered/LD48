using System;
using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.Movement
{
    [GameSystem]
    public class PaperMarioStyleAnimationSystem : GameSystem<PaperMarioStyleAnimationComponent>
    {
        public override void Register(PaperMarioStyleAnimationComponent component)
        {
            var twoDeeComp = component.GetComponent<TwoDeeMovementComponent>();
            twoDeeComp.Direction
                .Pairwise()
                .Where(DirectionChanged)
                .Throttle(TimeSpan.FromMilliseconds(component.AnimationDelay))
                .Select(pair => (pair.Current, component))
                .Subscribe(ChangeDirection)
                .AddTo(component);
        }

        private void ChangeDirection((Vector2 Current, PaperMarioStyleAnimationComponent component) data)
        {
            var (Current, component) = data;
            if (Current.x < 0)
            {
                component.AnimationDisposable?.Dispose();
                component.AnimationDisposable = Observable
                    .FromMicroCoroutine(_ => component.Turn(component.transform.localScale, new Vector3(-1, 1, 1)))
                    .Subscribe();
            }
            else
            {
                component.AnimationDisposable?.Dispose();
                component.AnimationDisposable = Observable
                    .FromMicroCoroutine(_ => component.Turn(component.transform.localScale, new Vector3(1, 1, 1)))
                    .Subscribe();
            }
        }

        private bool DirectionChanged(Pair<Vector2> pair)
        {
            return pair.Current.x < 0 && pair.Previous.x >= 0 || pair.Current.x > 0 && pair.Previous.x <= 0;
        }
    }
}
