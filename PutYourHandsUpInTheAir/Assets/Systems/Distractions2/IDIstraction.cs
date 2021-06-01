
namespace Systems.Distractions2
{
    public interface IDistraction
    {
        DistractionOutcome Update(DistractableComponent distractable);
    }
}
