using SystemBase;
using Systems.DistractionControl;
using Systems.Distractions2;
using Systems.Tourist;
using Systems.Tourist.States;

namespace Systems.Distractions
{
    [GameSystem(typeof(DistractionControlSystem))]
    public class ButterflyDistractionSystem : GameSystem<ButterflyDistractionTouristComponent>
    {
        public override void Register(ButterflyDistractionTouristComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            touristBrain.States.GoToState(new GoingToAttraction(component.InteractionPosition));

            var distractable = component.GetComponent<DistractableComponent>();
            distractable.DistractionType.Value = DistractionType.Butterfly;
        }
    }
}
