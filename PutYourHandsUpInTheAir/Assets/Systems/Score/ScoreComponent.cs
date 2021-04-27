using SystemBase;
using Systems.Tourist;
using UniRx;

namespace Systems.Score
{
    public class ScoreComponent : GameComponent
    {
        public ReactiveProperty<TouristDump[]> touristStats = new ReactiveProperty<TouristDump[]>();
    }
}