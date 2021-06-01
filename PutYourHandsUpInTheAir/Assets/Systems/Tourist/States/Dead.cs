using SystemBase.StateMachineBase;
using Systems.Distractions2;

namespace Systems.Tourist.States
{
    [NextValidStates(/*none, you are dead...*/)]
    public class Dead : BaseState<TouristBrainComponent>
    {
        public DistractionType KilledByDistractionType;

        public Dead(DistractionType killedByDistractionType)
        {
            KilledByDistractionType = killedByDistractionType;
        }
        public override void Enter(StateContext<TouristBrainComponent> context)
        {

        }
    }
}