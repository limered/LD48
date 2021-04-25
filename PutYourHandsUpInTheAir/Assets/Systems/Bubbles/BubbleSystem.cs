using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SystemBase;
using Systems.Tourist;
using Systems.Distractions;
using Systems.DistractionControl;


[GameSystem]
public class BubbleSystem : GameSystem<BubbleComponent, TouristBrainComponent>
{
    public override void Register(BubbleComponent component)
    {
        throw new System.NotImplementedException();
    }

    public override void Register(TouristBrainComponent component)
    {
        throw new System.NotImplementedException();
    }

    private void ShowPickingInterest(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[1];
    }

    private void ShowDistractionBubble(BubbleComponent component)
    {

        var distractionType = component.GetComponent<DistractedTouristComponent>().CurrentDistractionType;
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();

        switch (distractionType)
        {
            case DistractionType.None:
                spriteRenderer.sprite = component.Bubbles[0];
                break;
            case DistractionType.Tiger:
                spriteRenderer.sprite = component.Bubbles[2];
                break;
            default:
                HideBubble(component);
                break;
        }
    }

    private void ShowDeathBubble(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[0];
    }

    private void HideBubble(BubbleComponent component)
    {
        component.gameObject.SetActive(false);
    }
}
