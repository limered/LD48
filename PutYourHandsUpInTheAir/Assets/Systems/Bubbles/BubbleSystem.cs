using UnityEngine;
using UniRx;
using SystemBase;
using Systems.Tourist;
using Systems.Distractions;
using Systems.DistractionControl;
using Systems.Tourist.States;
using SystemBase.StateMachineBase;

[GameSystem]
public class BubbleSystem : GameSystem<TouristBrainComponent>
{
    public override void Register(TouristBrainComponent touristBrainComponent)
    {
        var bubbleComponent = touristBrainComponent.GetComponentInChildren<BubbleComponent>();

        touristBrainComponent.States.CurrentState
            .Subscribe(state =>
            {
                HandleTouristState(touristBrainComponent, bubbleComponent, state);
            })
            .AddTo(touristBrainComponent);
    }

    private void HandleTouristState(TouristBrainComponent touristBrainComponent, BubbleComponent bubbleComponent, BaseState<TouristBrainComponent> state)
    {
        if (state is PickingInterest)
        {
            ShowPickingInterest(bubbleComponent);
        }
        else if (state is GoingToAttraction 
                 || state is GoingBackToIdle)
        {
            ShowDistractionBubble(bubbleComponent, touristBrainComponent);
        }
        else if (state is Interacting)
        {
            ShowDistractionProgress(bubbleComponent, touristBrainComponent);
        }
        else if (state is Dead)
        {
            ShowDeathBubble(bubbleComponent);
        }
        else
        {
            ShowBubble(bubbleComponent, false);
        }
    }

    private void ShowPickingInterest(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[0];
        ShowBubble(component, true);
    }

    private void ShowDistractionBubble(BubbleComponent bubbleComponent, TouristBrainComponent touristBrainComponent)
    {
        var distracted = touristBrainComponent.GetComponent<DistractedTouristComponent>();
        if (!distracted) return;
        var distractionType = distracted.CurrentDistractionType;
        var spriteRenderer = bubbleComponent.gameObject.GetComponent<SpriteRenderer>();
        ShowBubble(bubbleComponent, true);

        if (distractionType is DistractionType.Tiger)
        {
            spriteRenderer.sprite = bubbleComponent.Bubbles[2];
        } else if (distractionType is DistractionType.Butterfly) {
            spriteRenderer.sprite = bubbleComponent.Bubbles[3];
        } else if (distractionType is DistractionType.Bus)
        {
            spriteRenderer.sprite = bubbleComponent.Bubbles[4];
        } else if (distractionType is DistractionType.Spider)
        {
            spriteRenderer.sprite = bubbleComponent.Bubbles[5];
        } else if (distractionType is DistractionType.Swamp)
        {
            spriteRenderer.sprite = bubbleComponent.Bubbles[6];
        } else if (distractionType is DistractionType.Money)
        {
            spriteRenderer.sprite = bubbleComponent.Bubbles[7];
        } else
        {
            ShowBubble(bubbleComponent, false);
        }
    }

    private void ShowDeathBubble(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[1];
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
        var distractedTourist = touristBrainComponent.GetComponent<DistractedTouristComponent>();
        var activatedColor = distractedTourist.ProgressColor;
        distractedTourist.DistractionProgress.Subscribe( progress => {
            spriteRenderer.color = Color32.Lerp(Color.white, activatedColor, progress);
        });
    }
}
