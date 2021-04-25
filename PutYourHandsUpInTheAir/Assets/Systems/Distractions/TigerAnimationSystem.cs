using System;
using System.Collections;
using System.Collections.Generic;
using SystemBase;
using Systems.DistractionControl;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;

[GameSystem]
public class TigerAnimationSystem : GameSystem<DistractionComponent>
{
    public override void Register(DistractionComponent component)
    {
        component.InteractionCollider
            .OnTriggerEnterAsObservable()
            .Subscribe(collider => StartWakeUpAnimation(collider, component))
            .AddToLifecycleOf(component);
    }

    private void StartWakeUpAnimation(Collider collider, DistractionComponent component)
    {
        var collidingObject = collider.gameObject;
        if (collidingObject.CompareTag("tourist"))
        {
            var animator = component.GetComponent<Animator>();
            animator.Play("TigerWakingUp_Head");
            animator.Play("TigerBody_Idle");
            animator.Play("TigerWakingUp_Tail");

            collidingObject.GetComponent<TouristBrainComponent>().States.CurrentState
            .Subscribe(state =>
            {
                HandleAnimationForState(component, collider, state);
            })
            .AddToLifecycleOf(component);
        }
    }

    private void HandleAnimationForState(DistractionComponent component, Collider collider, SystemBase.StateMachineBase.BaseState<TouristBrainComponent> state)
    {
        var collidingObject = collider.gameObject;
        if (collidingObject.CompareTag("tourist"))
        {
            var animator = component.GetComponent<Animator>();
            if (state is Dead)
            {
                animator.Play("TigerAttack_Head");
                animator.Play("TigerAttack_Body");
                animator.Play("TigerAttack_Tail");
            }
            else if (state is GoingBackToIdle)
            {
                animator.Play("TigerFallingAsleep_Head");
                animator.Play("TigerBody_Idle");
                animator.Play("TigerTail_Idle");
            }
        }
    }
}
