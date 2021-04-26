using SystemBase;
using Systems.Distractions;
using UniRx;
using Utils.Plugins;

namespace Systems.FirstRoom
{
    public class FirstRooSystem : GameSystem<FirstRoomComponent, TigerDistractionTouristComponent>
    {
        private readonly ReactiveProperty<FirstRoomComponent> _firstRoomComponent = new ReactiveProperty<FirstRoomComponent>();

        public override void Register(FirstRoomComponent component)
        {
            _firstRoomComponent.Value = component;
        }

        public override void Register(TigerDistractionTouristComponent component)
        {
            _firstRoomComponent.WhereNotNull()
                .Subscribe(first => TigerDistractionTriggered(first, component))
                .AddToLifecycleOf(component);
        }

        private void TigerDistractionTriggered(FirstRoomComponent firstRoom, TigerDistractionTouristComponent component)
        {
            throw new System.NotImplementedException();
        }
    }

    public class FirstRoomComponent : GameComponent
    {
    }
}
