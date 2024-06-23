using Systems.DistractionManagement;

namespace Systems.Distractions.Messages
{
    public class AbortDistractionAction
    {
        public AbortDistractionAction(DistractionOriginComponent distraction)
        {
            Distraction = distraction;
        }

        public DistractionOriginComponent Distraction { get; set; }
    }
}
