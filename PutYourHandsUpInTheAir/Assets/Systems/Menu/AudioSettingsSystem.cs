using System.Linq;
using SystemBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Plugins;

namespace Systems.Menu
{
    [GameSystem]
    public class AudioSettingsSystem : GameSystem<AudioSliderComponent, AudioSourceVolumeFromSettingsComponent>
    {
        private readonly FloatReactiveProperty _volume = new FloatReactiveProperty(0.5f);
        
        public override void Register(AudioSliderComponent component)
        {
            component.volume.Value = _volume.Value;
            var slider = component.GetComponent<Slider>();
            slider.value = _volume.Value;

            component.volume.Subscribe(v => _volume.Value = v).AddToLifecycleOf(component);
        }

        public override void Register(AudioSourceVolumeFromSettingsComponent component)
        {
            _volume.Subscribe(v => component.volume.Value = v).AddToLifecycleOf(component);

            var audioSources = component.GetComponents<AudioSource>();
            //we use the initially set volume on the audio source as max volume
            var maxVolumes = audioSources.Select(x => x.volume).ToArray();
            
            component.volume
                .Subscribe(v =>
                {
                    for (var i=0; i<audioSources.Length; i++)
                    {
                        audioSources[i].volume = maxVolumes[i] * v;
                    }
                })
                .AddToLifecycleOf(component);
        }
    }
}