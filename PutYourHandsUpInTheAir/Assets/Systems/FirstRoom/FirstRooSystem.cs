using System;
using System.Net.Mime;
using SystemBase;
using Systems.Distractions;
using Systems.Tourist;
using UniRx;
using Utils.Plugins;

namespace Systems.FirstRoom
{
    [GameSystem]
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
            var touristName = component.GetComponent<TouristBrainComponent>().touristName.Value;

            var touristText = "Hey! " + touristName + " is going to the Tiger! \n Please click on them to save their life!";

            firstRoom.TouristNameText.text = touristText;
            firstRoom.FirstRoomTextElement.SetActive(true);

            Observable.Timer(TimeSpan.FromSeconds(8))
                .Subscribe(_ => firstRoom.FirstRoomTextElement.SetActive(false))
                .AddToLifecycleOf(firstRoom);
        }
    }
}
