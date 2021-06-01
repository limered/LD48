namespace Systems.Distractions2.DistractionStrategies
{
    public class BusDistraction : IDIstraction
    {
        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Waiting;
        }
    }
}