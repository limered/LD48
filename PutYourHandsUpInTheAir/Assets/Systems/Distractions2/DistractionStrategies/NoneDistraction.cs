namespace Systems.Distractions2.DistractionStrategies
{
    public class NoneDistraction : IDistraction
    {
        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Alive;
        }
    }
}
