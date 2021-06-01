namespace Systems.Distractions2.DistractionStrategies
{
    public class BusDistraction : IDistraction
    {
        public void Init()
        {
        }

        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Waiting;
        }
    }
}