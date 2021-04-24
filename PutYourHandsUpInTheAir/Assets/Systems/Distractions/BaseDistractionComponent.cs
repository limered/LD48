using SystemBase;
using UniRx;

namespace Systems.Distractions
{
    public class BaseDistractionComponent : GameComponent
    {
        public ReactiveCommand StartDistraction = new ReactiveCommand();
    }
}