using SystemBase;
using Systems.Room;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Systems.World
{
    [GameSystem]
    public class WorldSystem : GameSystem<WorldComponent, RoomComponent>
    {
        public override void Register(WorldComponent component)
        {
            RegisterWaitable(component);
            LoadFirstLevel(component);
        }

        public override void Register(RoomComponent room)
        {
            WaitOn<WorldComponent>()
                .Subscribe(world => LoadNextLevelOnRoomNextState(world, room))
                .AddToLifecycleOf(room);
        }

        private void LoadFirstLevel(WorldComponent component)
        {
            var lvlPrefab = component.FirstLevel;
            LoadLevel(component, lvlPrefab);
        }

        private void LoadLevel(WorldComponent component, GameObject lvlPrefab)
        {
            component.CurrentLevelNr.Value++;
            var lvl = Object.Instantiate(lvlPrefab);
            component.CurrentLevel.Value = lvl;
            lvl.GetComponent<RoomComponent>();
        }

        private void LoadNextLevel(WorldComponent component)
        {
            Object.Destroy(component.CurrentLevel.Value);
            var lvlPrefab = component.EasyRooms[(int)(Random.value * component.EasyRooms.Length)];
            LoadLevel(component, lvlPrefab);
        }
        private void LoadNextLevelOnRoomNextState(WorldComponent world, RoomComponent room)
        {
            room.State.CurrentState
                .Where(state => state is RoomNext)
                .Subscribe(_ => LoadNextLevel(world))
                .AddToLifecycleOf(room);
        }
    }
}
