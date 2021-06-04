
namespace Systems.Distractions2
{
    public interface IDistraction
    {
        void Init(DistractableComponent distractable);
        DistractionOutcome Update(DistractableComponent distractable);
    }
}
