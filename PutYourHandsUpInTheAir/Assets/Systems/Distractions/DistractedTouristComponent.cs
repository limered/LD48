using SystemBase;
using Systems.DistractionControl;
using UniRx;

namespace Systems.Distractions
{
    public class DistractedTouristComponent : GameComponent
    {
        public DistractionType CurrentDistractionType;
        public FloatReactiveProperty DistractionProgress = new FloatReactiveProperty(0);
    }
}
