namespace Systems.Distractions2.DistractionStrategies
{
    public class BusDistraction : IDistraction
    {
        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Waiting;
        }
    }
}