using System;
using SystemBase;
using Systems.Distractions;
using Systems.Tourist;
using UniRx;
using Utils.Plugins;

namespace Systems.FirstRoom
{
    [GameSystem]
    public class FirstRoomSystem : GameSystem<FirstRoomComponent, DistractableComponent>
    {
        private readonly ReactiveProperty<FirstRoomComponent> _firstRoomComponent = new ReactiveProperty<FirstRoomComponent>();

        public override void Register(FirstRoomComponent component)
        {
            _firstRoomComponent.Value = component;
        }

        public override void Register(DistractableComponent component)
        {
            component.DistractionType
                .Where(_ => _firstRoomComponent.Value != null)
                .Where(type => type == DistractionType.Tiger)
                .Subscribe(_ => TigerDistractionTriggered(_firstRoomComponent.Value, component))
                .AddTo(component);
        }

        private void TigerDistractionTriggered(FirstRoomComponent firstRoom, DistractableComponent component)
        {
            var touristBrain = component.GetComponent<TouristBrainComponent>();
            if (!touristBrain) return;

            var touristText = "Hey! " + touristBrain.touristName.Value + " is going to the Tiger! \n Please click on the tiger to save their life!";

            if (!firstRoom || !firstRoom.TouristNameText || !firstRoom.FirstRoomTextElement) return;

            firstRoom.TouristNameText.text = touristText;
            firstRoom.FirstRoomTextElement.SetActive(true);

            Observable.Timer(TimeSpan.FromSeconds(8))
                .Subscribe(_ => firstRoom.FirstRoomTextElement.SetActive(false))
                .AddToLifecycleOf(firstRoom);
        }
    }
}
