using SystemBase;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Menu
{
    public class AudioSliderComponent : GameComponent
    {
        public FloatReactiveProperty volume = new FloatReactiveProperty();

        public void SetVolume(float v)
        {
            volume.Value = v;
        }
    }
}