using SystemBase;
using Systems.DistractionManagement;
using Systems.Distractions;
using Systems.GameMessages.Messages;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;

namespace Systems.TigerAnimation
{
    [GameSystem]
    public class TigerAnimationSystem : GameSystem<DistractionOriginComponent>
    {

        public override void Register(DistractionOriginComponent originComponent)
        {
            if (originComponent.DistractionType != DistractionType.Tiger) return;

            originComponent.TouristInteractionCollider
                .OnTriggerEnterAsObservable()
                .Where(_ => originComponent.GetComponent<TigerAnimationComponent>().CurrentState == TigerState.Sleeping)
                .Subscribe(collider => StartWakeUpAnimation(collider, originComponent))
                .AddToLifecycleOf(originComponent);

            MessageBroker.Default.Receive<DistractionOutcomeDeadAction>()
                .Subscribe(msg =>
                {
                    if (originComponent == msg.Origin)
                    {
                        Kill(originComponent);
                    }
                })
                .AddTo(originComponent);
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
            }
        }

        private void Kill(DistractionOriginComponent originComponent)
        {
            var animator = originComponent.GetComponent<Animator>();
            var tigerComponent = originComponent.GetComponent<TigerAnimationComponent>();

            tigerComponent.CurrentState = TigerState.Kill;
            animator.Play("TigerAttack_Head");
            animator.Play("TigerAttack_Body");
            animator.Play("TigerAttack_Tail");
            tigerComponent.GetComponent<AudioSource>().Play();
        }

        private void GoBackToSleep(DistractionOriginComponent originComponent)
        {
            var animator = originComponent.GetComponent<Animator>();
            var tigerComponent = originComponent.GetComponent<TigerAnimationComponent>();
            tigerComponent.CurrentState = TigerState.Sleeping;
            animator.Play("TigerFallingAsleep_Head");
            animator.Play("TigerBody_Idle");
            animator.Play("TigerTail_Idle");
        }
    }
}
