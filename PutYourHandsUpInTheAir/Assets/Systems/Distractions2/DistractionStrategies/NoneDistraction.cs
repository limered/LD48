namespace Systems.Distractions2.DistractionStrategies
{
    public class NoneDistraction : IDIstraction
    {
        public DistractionOutcome Update(DistractableComponent distractable)
        {
            return DistractionOutcome.Alive;
        }
    }
}
