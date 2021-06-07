using SystemBase;
using Systems.Distractions;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Plugins;

namespace Systems.Player.TouristInteraction
{
    [GameSystem]
    public class PlayerDistractionInteractionSystem : GameSystem<PlayerComponent>
    {
        private readonly ReactiveProperty<PlayerComponent> _currentPlayer = new ReactiveProperty<PlayerComponent>();
        private readonly int distractionLayer = LayerMask.NameToLayer("Distraction");

        public override void Register(PlayerComponent player)
        {
            _currentPlayer.Value = player;

            SystemUpdate(player)
                .Subscribe(CheckDistractionForClick)
                .AddToLifecycleOf(player);
        }

        private void CheckDistractionForClick(PlayerComponent player)
        {
            if (!IsLeftMouseClicked()) return;
            if (!HasClickedOnDistraction(out var hit)) return;

            var distraction = hit.transform.GetComponent<DistractionOriginComponent>();
            if (!distraction) return;

            player.TargetedDistraction.Value = distraction;
            player.TargetVector = distraction.transform.position;
        }

        private bool HasClickedOnDistraction(out RaycastHit hit)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << distractionLayer);
        }

        private static bool IsLeftMouseClicked()
        {
            return Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
        }
    }
}
