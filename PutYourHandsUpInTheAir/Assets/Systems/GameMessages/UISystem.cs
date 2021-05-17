using System;
using System.Collections;
using System.Collections.Generic;
using SystemBase;
using Systems.Tourist;
using UniRx;
using UniRx.Triggers;

[GameSystem]
public class UISystem : GameSystem<UIComponent>
{
    private float sec = 80f;

    public override void Register(UIComponent component)
    {
        MessageBroker.Default.Receive<ShowDeadPersonAction>()
            .Subscribe(msg =>
            {
                ResetTime();
                var text = component.Message.Text;
                var face = component.Message.Image;

                text.text = msg.TouristName + " died.";
                var faces = component.GetComponentInParent<TouristConfigComponent>().topParts;
                face.sprite = faces[msg.TouristFaceIndex];

                component.Message.gameObject.SetActive(true);

            })
            .AddTo(component);

        component.UpdateAsObservable().Subscribe(_ =>
        {
            if(sec > 0)
            {
                sec--;
            } else
            {
                component.Message.gameObject.SetActive(false);
                ResetTime();
            }
        });
    }

    private void ResetTime()
    {
        sec = 80f;
    }
}
