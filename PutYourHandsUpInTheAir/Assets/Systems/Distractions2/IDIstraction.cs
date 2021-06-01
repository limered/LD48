
namespace Systems.Distractions2
{
    public interface IDistraction
    {
        void Init();
        DistractionOutcome Update(DistractableComponent distractable);
    }
}
