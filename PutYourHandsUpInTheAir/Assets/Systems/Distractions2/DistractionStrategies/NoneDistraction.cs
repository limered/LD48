namespace Systems.Distractions2.DistractionStrategies
{
    public class NoneDistraction : IDistraction
    {
        public void Init(DistractableComponent distractable)
        {
        }

        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Alive;
        }
    }
}
