namespace Systems.Distractions2.DistractionStrategies
{
    public class SpiderDistraction : IDistraction
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
