using SystemBase;
using Systems.DistractionControl;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Plugins;

namespace Systems.Player.TouristInteraction
{
    [GameSystem]
    public class PlayerTouristInteractionSystem : GameSystem<PlayerComponent>
    {
        private readonly ReactiveProperty<PlayerComponent> _currentPlayer = new ReactiveProperty<PlayerComponent>();
        private readonly int distractionLayer = LayerMask.NameToLayer("Distraction");

        public override void Register(PlayerComponent player)
        {
            _currentPlayer.Value = player;

            SystemUpdate(player)
                .Subscribe(CheckTouristForClick)
                .AddToLifecycleOf(player);
        }

        private void CheckTouristForClick(PlayerComponent player)
        {
            if (!Input.GetMouseButtonDown((int) MouseButton.LeftMouse)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << distractionLayer)) return;

            var touristBrain = hit.transform.GetComponent<DistractionComponent>();

            player.TargetedDistraction.Value = touristBrain;
        }
    }
}
