namespace Systems.Distractions2.DistractionStrategies
{
    public class MoneyDistraction : IDIstraction
    {
        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Waiting;
        }
    }
}
