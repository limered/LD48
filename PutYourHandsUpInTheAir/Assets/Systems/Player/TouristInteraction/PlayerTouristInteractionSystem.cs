using SystemBase;
using Systems.Tourist;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Plugins;

namespace Systems.Player.TouristInteraction
{
    [GameSystem]
    public class PlayerTouristInteractionSystem : GameSystem<PlayerComponent, TouristBrainComponent>
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

                player.LastTargetedTourist.Value = player.TargetedTourist.Value;
                player.TargetedTourist.Value = hit.transform.GetComponent<TouristBrainComponent>();
            }
        }

        public override void Register(TouristBrainComponent component)
        {
            _currentPlayer.WhereNotNull()
                .Subscribe(player =>
                {
                    player.OnTriggerEnterAsObservable()
                        .Where(coll => coll.gameObject.GetComponent<TouristBrainComponent>())
                        .Select(coll => (coll, player))
                        .Subscribe(TouristCollidesWithPlayer)
                        .AddToLifecycleOf(component);

                    player.OnTriggerExitAsObservable()
                        .Where(coll => coll.gameObject.GetComponent<TouristBrainComponent>())
                        .Select(coll => (coll, player))
                        .Subscribe(PlayerLeavesTourist)
                        .AddToLifecycleOf(component);
                })
                .AddToLifecycleOf(component);
        }

        private void PlayerLeavesTourist((Collider coll, PlayerComponent player) obj)
        {
            Object.Destroy(obj.coll.gameObject.GetComponent<IsNearPlayerComponent>());
        }

        private void TouristCollidesWithPlayer((Collider coll, PlayerComponent player) obj)
        {
            var comp = obj.coll.gameObject.AddComponent<IsNearPlayerComponent>();
            comp.Player = obj.player;
        }
    }
}
