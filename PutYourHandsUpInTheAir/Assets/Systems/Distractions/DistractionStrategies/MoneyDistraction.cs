namespace Systems.Distractions.DistractionStrategies
{
    public class MoneyDistraction : IDistraction
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
