using SystemBase;
using Systems.Tourist;
using UniRx;
using Utils.Plugins;

namespace Systems.Player.TouristInteraction
{
    [GameSystem]
    public class PlayerTouristInteractionSystem : GameSystem<PlayerComponent, TouristBrainComponent>
    {
        private readonly ReactiveProperty<PlayerComponent> _currentPlayer = new ReactiveProperty<PlayerComponent>();

        public override void Register(PlayerComponent component)
        {
            _currentPlayer.Value = component;
        }

        public override void Register(TouristBrainComponent component)
        {
            //_currentPlayer
            //    .WhereNotNull()
            //    .Subscribe(player => )
            //    .AddToLifecycleOf(component);
        }
    }
}
