using Systems.DistractionControl;
using Systems.Tourist;

namespace Systems.Distractions2.Messages
{
    public class DistractTouristAction
    {
        public DistractTouristAction(DistractionOriginComponent distractionOriginOrigin, 
            TouristBrainComponent tourist)
        {
            DistractionOriginOrigin = distractionOriginOrigin;
            Tourist = tourist;
        }

        private DistractionOriginComponent DistractionOriginOrigin { get; }
        public TouristBrainComponent Tourist { get; }

        public T GetOrigin<T>() where T : DistractionOriginComponent, new()
        {
            return (T)DistractionOriginOrigin;
        }
    }
}
