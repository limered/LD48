using SystemBase;
using Systems.Distractions;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;

[GameSystem]
public class TigerAnimationSystem : GameSystem<DistractionOriginComponent>
{
    public override void Register(DistractionOriginComponent originComponent)
    {
        originComponent.InteractionCollider
            .OnTriggerEnterAsObservable()
            .Where(_ => originComponent.GetComponent<TigerAnimationComponent>().CurrentState == TigerState.Sleeping)
            .Subscribe(collider => StartWakeUpAnimation(collider, originComponent))
            .AddToLifecycleOf(originComponent);
    }

    private void StartWakeUpAnimation(Collider collider, DistractionOriginComponent originComponent)
    {
        var collidingObject = collider.gameObject;
        if (collidingObject.CompareTag("tourist"))
        {
            var tigerComponent = originComponent.GetComponent<TigerAnimationComponent>();
            tigerComponent.CurrentState = TigerState.Awake;

            var animator = originComponent.GetComponent<Animator>();

            animator.Play("TigerWakingUp_Head");
            animator.Play("TigerBody_Idle");
            animator.Play("TigerWakingUp_Tail");

            collidingObject.GetComponent<TouristBrainComponent>().StateContext.CurrentState
            .Subscribe(state =>
            {
                HandleAnimationForState(originComponent, collider, state);
            })
            .AddToLifecycleOf(originComponent);
        }
    }

    private void HandleAnimationForState(DistractionOriginComponent originComponent, Collider collider, SystemBase.StateMachineBase.BaseState<TouristBrainComponent> state)
    {
        var collidingObject = collider.gameObject;
        if (collidingObject.CompareTag("tourist"))
        {
            var animator = originComponent.GetComponent<Animator>();
            var tigerComponent = originComponent.GetComponent<TigerAnimationComponent>();

            if (state is Dead)
            {
                tigerComponent.CurrentState = TigerState.Kill;
                animator.Play("TigerAttack_Head");
                animator.Play("TigerAttack_Body");
                animator.Play("TigerAttack_Tail");
                tigerComponent.GetComponent<AudioSource>().Play();
            }
            else if (state is GoingBackToIdle)
            {
                tigerComponent.CurrentState = TigerState.Sleeping;
                GoBackToSleep(animator);
            }
        }
    }

    private void GoBackToSleep(Animator animator)
    {
        animator.Play("TigerFallingAsleep_Head");
        animator.Play("TigerBody_Idle");
        animator.Play("TigerTail_Idle");
    }
}
