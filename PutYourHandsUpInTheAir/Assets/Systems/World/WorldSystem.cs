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

            var lvlPrefab = (component.PlayTestMode) 
                ? component.PlayTestRoomPrefab 
                : component.FirstLevel;
            LoadLevel(component, lvlPrefab);
        }

        private void LoadLevel(WorldComponent world, GameObject lvlPrefab)
        {
            

            world.CurrentLevelNr.Value++;
            var lvl = Object.Instantiate(lvlPrefab);
            world.CurrentLevel.Value = lvl;
            var room = lvl.GetComponent<RoomComponent>();


        }

        private void LoadNextLevel(WorldComponent component)
        {
            Object.Destroy(component.CurrentLevel.Value);
            if (component.PlayTestMode)
            {
                var lvlPrefab = component.PlayTestRoomPrefab;
                LoadLevel(component, lvlPrefab);
                
            }
            else if (component.CurrentLevelNr.Value >= component.MaxLevelCount)
            {
                var lvlPrefab = component.LastLevel;
                LoadLevel(component, lvlPrefab);
            }
            else
            {
                var lvlPrefab = component.EasyRooms[(int)(Random.value * component.EasyRooms.Length)];
                LoadLevel(component, lvlPrefab);
            }
            
        }
        private void LoadNextLevelOnRoomNextState(WorldComponent world, RoomComponent room)
        {
            var fadeComp = room.GetComponentInChildren<RoomFadeOutComponent>();
            if (fadeComp)
            {
                if (world.CurrentLevelNr.Value > world.MaxLevelCount)
                {
                    fadeComp.FadeToColor = new Color(0, 0, 0, 0);
                }
                else
                {
                    var fadeToAlpha = (float) world.CurrentLevelNr.Value / (float) world.MaxLevelCount;
                    fadeComp.FadeToColor = Color.Lerp(new Color(0, 0, 0, 0), world.FinalJungleColor, fadeToAlpha);
                }
            }

            if (!world.PlayTestMode && world.CurrentLevelNr.Value > world.MaxLevelCount) return;

            room.State.CurrentState
                .Where(state => state is RoomNext)
                .Subscribe(_ => LoadNextLevel(world))
                .AddToLifecycleOf(room);
        }
    }
}
