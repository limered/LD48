namespace Systems.Distractions2.DistractionStrategies
{
    public class BusDistraction : IDistraction
    {
        public void Init(DistractableComponent distractable)
        {
        }

        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Waiting;
        }
    }
}