using SystemBase;
using Systems.Tourist;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using Utils.Plugins;

namespace Systems.Player.TouristInteraction
{
    [GameSystem]
    public class PlayerTouristInteractionSystem : GameSystem<PlayerComponent>
    {
        private readonly ReactiveProperty<PlayerComponent> _currentPlayer = new ReactiveProperty<PlayerComponent>();

        public override void Register(PlayerComponent player)
        {
            _currentPlayer.Value = player;

            SystemUpdate(player)
                .Subscribe(CheckTouristForClick)
                .AddToLifecycleOf(player);
        }

        private void CheckTouristForClick(PlayerComponent player)
        {
            var touristLayer = LayerMask.NameToLayer("Tourist");

            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << touristLayer)) return;

                player.LastTargetetTourist = player.TargetedTourist;
                player.TargetedTourist = hit.transform.GetComponent<TouristBrainComponent>();
            }
        }
    }
}
