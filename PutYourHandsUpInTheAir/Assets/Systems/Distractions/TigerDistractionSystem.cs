using SystemBase;
using Systems.DistractionControl;
using Systems.Distractions2;
using Systems.Tourist;
using Systems.Tourist.States;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class TigerDistractionSystem : GameSystem<TigerDistractionTouristComponent>
    {
        public override void Register(TigerDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition.position));

            var distractable = component.GetComponent<DistractableComponent>();
            distractable.DistractionType.Value = DistractionType.Tiger;
        }
    }
}
