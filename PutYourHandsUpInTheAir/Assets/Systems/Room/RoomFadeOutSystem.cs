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

        private float StartStamp;
        private float EndStamp;

        public override void Register(RoomFadeOutComponent component)
        {
            PaintItBlack(component);

            SystemUpdate(component)
                .Subscribe(FadeOutOrIn)
                .AddToLifecycleOf(component);

            component.Room.State.CurrentState
                .WhereNotNull()
                .Where(state => state is RoomWalkIn)
                .Subscribe(_ => StartStamp = Time.timeSinceLevelLoad)
                .AddToLifecycleOf(component);

            component.Room.State.CurrentState
                .WhereNotNull()
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
                var alpha = (Time.timeSinceLevelLoad - StartStamp) / comp.FadeTime;
                comp.GetComponent<Image>().color = Color.Lerp(Hidden, comp.FadeToColor, alpha);
            }
            else if (comp.Room.State.CurrentState.Value is RoomDestroy)
            {
                var alpha = (Time.timeSinceLevelLoad - EndStamp) / comp.FadeTime;
                comp.GetComponent<Image>().color = Color.Lerp(comp.FadeToColor, Hidden, alpha);

                if (alpha > 1f)
                {
                    comp.Room.State.GoToState(new RoomNext());
                }
            }
        }
    }
}
