using SystemBase;
using Systems.Room;
using UniRx;
using Utils.Plugins;

namespace Systems.RoomExitBlock
{
    [GameSystem]
    public class RoomExitBlockSystem : GameSystem<RoomExitBlockComponent, RoomComponent>
    {
        private readonly ReactiveProperty<RoomExitBlockComponent> _blockComponent = new ReactiveProperty<RoomExitBlockComponent>();

        public override void Register(RoomComponent component)
        {
            _blockComponent.WhereNotNull().Subscribe(roomExitBlockComponent =>
                {
                    roomExitBlockComponent.spriteRenderer.enabled = true;
                    var divider = 1f / roomExitBlockComponent.sprites.Length;
                    component.RoomTimeProgress
                        .Subscribe(roomProgress => UpdateBlockedPath(roomProgress, divider, roomExitBlockComponent))
                        .AddToLifecycleOf(component);
                    _blockComponent.Value = null;
                })
                .AddToLifecycleOf(component);
        }

        public override void Register(RoomExitBlockComponent component)
        {
            _blockComponent.Value = component;
        }

        private static void UpdateBlockedPath(float roomProgress, float divider,
            RoomExitBlockComponent roomExitBlockComponent)
        {
            var progressIndex = (int) (roomProgress / divider);
            if (progressIndex >= roomExitBlockComponent.sprites.Length)
            {
                roomExitBlockComponent.spriteRenderer.enabled = false;
                roomExitBlockComponent.particles.Play();
                return;
            }

            if (roomExitBlockComponent.ActiveSpriteIndex != progressIndex)
            {
                roomExitBlockComponent.particles.Play();
                roomExitBlockComponent.ActiveSpriteIndex = progressIndex;
            }

            roomExitBlockComponent.spriteRenderer.sprite = roomExitBlockComponent.sprites[progressIndex];
        }
    }
}