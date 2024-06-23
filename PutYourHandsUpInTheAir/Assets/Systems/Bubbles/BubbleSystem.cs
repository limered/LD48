using UnityEngine;
using UniRx;
using Systems.Tourist;
using Systems.Tourist.States;
using SystemBase.StateMachineBase;
using SystemBase;
using Systems.Distractions;

[GameSystem]
public class BubbleSystem : GameSystem<TouristBrainComponent>
{
    public override void Register(TouristBrainComponent touristBrainComponent)
    {
        var bubbleComponent = touristBrainComponent.GetComponentInChildren<BubbleComponent>();

        touristBrainComponent.StateContext.CurrentState
            .Subscribe(state =>
            {
                HandleTouristState(touristBrainComponent, bubbleComponent, state);
            })
            .AddTo(touristBrainComponent);
    }

    private void HandleTouristState(TouristBrainComponent touristBrainComponent, BubbleComponent bubbleComponent, BaseState<TouristBrainComponent> state)
    {
        switch (state)
        {
            case PickingInterest _:
                ShowPickingInterest(bubbleComponent);
                break;
            case GoingToDistraction _:
            case GoingBackToIdle _:
                ShowDistractionBubble(bubbleComponent, touristBrainComponent);
                break;
            case Interacting _:
                ShowDistractionProgress(bubbleComponent, touristBrainComponent);
                break;
            case Dead _:
                ShowDeathBubble(bubbleComponent);
                break;
            case Paying _:
                ShowPayingBubble(bubbleComponent);
                break;
            default:
                ShowBubble(bubbleComponent, false);
                break;
        }
    }

    private void ShowPayingBubble(BubbleComponent bubbleComponent)
    {
        var spriteRenderer = bubbleComponent.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = bubbleComponent.Bubbles[7];
        ShowBubble(bubbleComponent, true);
    }

    private void ShowPickingInterest(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[0];
        ShowBubble(component, true);
    }

    private void ShowDistractionBubble(BubbleComponent bubbleComponent, TouristBrainComponent touristBrainComponent)
    {
        var distracted = touristBrainComponent.GetComponent<DistractableComponent>();
        var distractionType = distracted.DistractionType.Value;
        var spriteRenderer = bubbleComponent.gameObject.GetComponent<SpriteRenderer>();
        
        switch (distractionType)
        {
            case DistractionType.Tiger:
                spriteRenderer.sprite = bubbleComponent.Bubbles[2];
                ShowBubble(bubbleComponent, true);
                break;
            case DistractionType.Butterfly:
                spriteRenderer.sprite = bubbleComponent.Bubbles[3];
                ShowBubble(bubbleComponent, true);
                break;
            case DistractionType.Spider:
                spriteRenderer.sprite = bubbleComponent.Bubbles[5];
                ShowBubble(bubbleComponent, true);
                break;
            case DistractionType.Swamp:
                spriteRenderer.sprite = bubbleComponent.Bubbles[6];
                ShowBubble(bubbleComponent, true);
                break;
            default:
                ShowBubble(bubbleComponent, false);
                break;
        }
    }

    private void ShowDeathBubble(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[1];
        ShowBubble(component, true);
    }

    private void ShowBubble(BubbleComponent component, bool show)
    {
        component.gameObject.SetActive(show);
        if(!show)
        {
            var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.white;
        }
    }

    private void ShowDistractionProgress(BubbleComponent bubbleComponent, TouristBrainComponent touristBrainComponent)
    {
        var spriteRenderer = bubbleComponent.gameObject.GetComponent<SpriteRenderer>();
        var distractedTourist = touristBrainComponent.GetComponent<DistractableComponent>();
        var activatedColor = Color.red;
        distractedTourist.DistractionProgress.Subscribe( progress => {
            spriteRenderer.color = Color32.Lerp(Color.white, activatedColor, progress);
        });
    }
}
