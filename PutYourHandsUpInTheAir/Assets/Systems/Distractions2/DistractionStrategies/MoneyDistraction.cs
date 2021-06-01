namespace Systems.Distractions2.DistractionStrategies
{
    public class MoneyDistraction : IDistraction
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
