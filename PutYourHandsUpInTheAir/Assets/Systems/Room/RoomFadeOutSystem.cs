using SystemBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Plugins;

namespace Systems.Room
{
    [GameSystem]
    public class RoomFadeOutSystem : GameSystem<RoomFadeOutComponent>
    {
        private readonly Color Hidden = new Color(0,0,0,1);
        private readonly Color Show = new Color(0,0,0,0);

        private float StartStamp;
        private float EndStamp;

        public override void Register(RoomFadeOutComponent component)
        {
            PaintItBlack(component);

            SystemUpdate(component)
                .Subscribe(FadeOutOrIn)
                .AddToLifecycleOf(component);

            component.Room.State.CurrentState
                .Where(state => state is RoomWalkIn)
                .Subscribe(_ => StartStamp = Time.timeSinceLevelLoad)
                .AddToLifecycleOf(component);

            component.Room.State.CurrentState
                .Where(state => state is RoomDestroy)
                .Subscribe(_ => EndStamp = Time.timeSinceLevelLoad)
                .AddToLifecycleOf(component);
        }

        private void PaintItBlack(Component component)
        {
            component.GetComponent<Image>().color = Hidden;
        }

        private void FadeOutOrIn(RoomFadeOutComponent comp)
        {
            if (comp.Room.State.CurrentState.Value is RoomWalkIn)
            {
                var alpha = 1 - comp.FadeTime / (Time.timeSinceLevelLoad - StartStamp);
                comp.GetComponent<Image>().color = Color.Lerp(Hidden, Show, alpha);
            }
            else if (comp.Room.State.CurrentState.Value is RoomDestroy)
            {
                var alpha = 1 - comp.FadeTime / (Time.timeSinceLevelLoad - EndStamp);
                comp.GetComponent<Image>().color = Color.Lerp(Show, Hidden, alpha);
            }
        }
    }
}
