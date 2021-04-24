using SystemBase;
using UniRx;

namespace Systems.DistractionControl
{
    public class DistractionComponent : GameComponent
    {
        public DistractionType DistractionType;

        public ReactiveCommand StartDistraction = new ReactiveCommand();
    }
}