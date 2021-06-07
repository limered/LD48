
namespace Systems.Distractions
{
    public interface IDistraction
    {
        void Init(DistractableComponent distractable);
        DistractionOutcome Update(DistractableComponent distractable);
    }
}
