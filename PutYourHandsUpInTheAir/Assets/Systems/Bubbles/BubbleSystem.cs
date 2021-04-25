using UnityEngine;
using UniRx;
using SystemBase;
using Systems.Tourist;
using Systems.Distractions;
using Systems.DistractionControl;
using Systems.Tourist.States;

[GameSystem]
public class BubbleSystem : GameSystem<BubbleComponent, TouristBrainComponent>
{
    public override void Register(BubbleComponent component)
    {
        RegisterWaitable(component);
    }

    public override void Register(TouristBrainComponent component)
    {
        WaitOn<BubbleComponent>()
            .Subscribe(bubbleComponent => Register(component, bubbleComponent))
            .AddTo(component);
    }

    public void Register(TouristBrainComponent touristBrainComponent, BubbleComponent bubbleComponent)
    {
        touristBrainComponent.States.CurrentState.Subscribe(state =>
        {
            if (state is PickingInterest)
            {
                ShowPickingInterest(bubbleComponent);
            } else if (state is GoingToAttraction)
            {
                ShowDistractionBubble(bubbleComponent, touristBrainComponent);
            } else if (state is Interacting)
            {
                ShowDistractionProgress(bubbleComponent, touristBrainComponent);
            } else if (state is Dead)
            {
                ShowDeathBubble(bubbleComponent);
            } else
            {
                ShowBubble(bubbleComponent, false);
            }
        });
    }

    private void ShowPickingInterest(BubbleComponent component)
    {
        var spriteRenderer = component.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = component.Bubbles[0];
        ShowBubble(component, true);
    }

    private void ShowDistractionBubble(BubbleComponent bubbleComponent, TouristBrainComponent touristBrainComponent)
    {
        var distractionType = touristBrainComponent.GetComponent<DistractedTouristComponent>().CurrentDistractionType;
        var spriteRenderer = bubbleComponent.gameObject.GetComponent<SpriteRenderer>();
        ShowBubble(bubbleComponent, true);

        if (distractionType is DistractionType.Tiger)
        {
            spriteRenderer.sprite = bubbleComponent.Bubbles[2];
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
        if(show)
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
