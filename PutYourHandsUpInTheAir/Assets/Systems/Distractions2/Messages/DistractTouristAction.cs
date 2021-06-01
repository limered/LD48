using Systems.DistractionControl;

namespace Systems.Distractions2.Messages
{
    public class DistractTouristAction
    {
        public DistractTouristAction(DistractionOriginComponent distractionOriginOrigin)
        {
            DistractionOriginOrigin = distractionOriginOrigin;
        }

        private DistractionOriginComponent DistractionOriginOrigin { get; }

        public T GetOrigin<T>() where T : DistractionOriginComponent, new()
        {
            return (T)DistractionOriginOrigin;
        }
    }
}
