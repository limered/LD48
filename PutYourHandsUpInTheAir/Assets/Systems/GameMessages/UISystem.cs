using StrongSystems.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemBase;
using Systems.Tourist;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[GameSystem]
public class UISystem : GameSystem<UIComponent>
{
    private float sec = 120f;
    private bool showing = false;

    public override void Register(UIComponent component)
    {
        MessageBroker.Default.Receive<ShowDeadPersonAction>()
            .Subscribe(msg =>
            {
                ResetTime();
                PrepareMessage(msg, component);
                "man-dying".Play();

                if (!showing)
                {
                    ShowMessage(component.Message.gameObject, true);
                }

            })
            .AddTo(component);

        component.UpdateAsObservable().Subscribe(_ =>
        {
            if(sec > 0)
            {
                sec--;
            } else
            {
                ShowMessage(component.Message.gameObject, false);
                ResetTime();
            }
        });
    }

    private void ResetTime()
    {
        sec = 80f;
    }

    private void PrepareMessage(ShowDeadPersonAction msg, UIComponent component)
    {
        var text = component.Message.Text;
        text.text = msg.TouristName + " died.";

        var faces = component.GetComponentInParent<TouristConfigComponent>().topParts;
        var face = component.Message.Image;
        face.sprite = faces[msg.TouristFaceIndex];

        var distractions = component.Message.Distractions;
        var distraction = component.Message.Distraction;
        distraction.sprite = distractions[msg.DistractionIndex];
    }

    private void ShowMessage(GameObject gameObject, bool show)
    {
        showing = show;
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("show", show);
    }
}
