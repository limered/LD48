namespace Systems.Distractions2.DistractionStrategies
{
    public class SpiderDistraction : IDIstraction
    {
        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Waiting;
        }
    }
}
