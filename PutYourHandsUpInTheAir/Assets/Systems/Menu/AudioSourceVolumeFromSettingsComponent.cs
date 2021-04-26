using SystemBase;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Menu
{
    public class AudioSourceVolumeFromSettingsComponent : GameComponent
    {
        public FloatReactiveProperty volume = new FloatReactiveProperty(1f);
    }
}