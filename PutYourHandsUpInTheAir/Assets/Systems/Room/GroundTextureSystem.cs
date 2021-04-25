using SystemBase;
using UnityEngine;

namespace Systems.Room
{
    [GameSystem]
    public class GroundTextureSystem : GameSystem<GroundComponent>
    {
        public override void Register(GroundComponent component)
        {
            var renderer = component.GetComponent<SpriteRenderer>();
            renderer.sprite = component.GroundTextures[(int) (Random.value * component.GroundTextures.Length)];
        }
    }
}
