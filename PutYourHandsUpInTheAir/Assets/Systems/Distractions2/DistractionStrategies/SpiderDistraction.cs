namespace Systems.Distractions2.DistractionStrategies
{
    public class SpiderDistraction : IDistraction
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
