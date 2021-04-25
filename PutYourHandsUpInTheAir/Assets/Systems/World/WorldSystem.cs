using SystemBase;
using UnityEngine;

namespace Systems.World
{
    [GameSystem]
    public class WorldSystem : GameSystem<WorldComponent>
    {
        public override void Register(WorldComponent component)
        {
            LoadFirstLevel(component);
        }

        private void LoadFirstLevel(WorldComponent component)
        {
            var lvlPrefab = component.EasyRooms[0];
            Object.Instantiate(lvlPrefab);
        }
    }
}
