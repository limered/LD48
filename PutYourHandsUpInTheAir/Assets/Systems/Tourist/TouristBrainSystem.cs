using System;
using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Movement;
using Systems.Tourist.States;
using Assets.Utils.Math;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristBrainSystem : GameSystem<TouristBrainComponent>
    {
        public override void Register(TouristBrainComponent component)
        {
            var movement = component.GetComponent<MovementComponent>();

            component.States.CurrentState
                .LogOnNext(state => $"{component.touristName}: {state}")
                .Do(state => component.debugCurrentState = $"{state}")
                .Subscribe(state =>
                {
                    if (state is GoingIntoLevel) //TODO: do we need this state?
                    {
                        //this prevents a deadlock when setting the state directly
                        Observable.Timer(TimeSpan.FromSeconds(0))
                            .Subscribe(_ =>
                                component.States.GoToState(
                                    new GoingBackToIdle(Random.insideUnitCircle /*<- gather around this point*/)))
                            .AddTo(state);
                    }
                    else if (state is GoingBackToIdle goingIdle)
                    {
                        GoingToIdlePosition(goingIdle, component, movement);
                    }
                    else if (state is Idle idle)
                    {
                        Idle(idle, component, movement);
                    }
                    else if (state is PickingInterest)
                    {
                        //TODO: show some kind of thinking process (DistractionControlSystem)
                    }
                    else if (state is GoingToAttraction attraction)
                    {
                        GoingToAttraction(attraction, component, movement);
                    }
                    else if (state is Interacting interacting)
                    {
                        Interacting(interacting, component, movement);
                    }
                    else if (state is Dead)
                    {
                        movement.Direction.Value = Vector2.zero;
                        // Object.Destroy(movement);
                        if (component.GetComponent<Collider>()) Object.Destroy(component.GetComponent<Collider>());
                        var body = component.GetComponent<TouristBodyComponent>();
                        if (body)
                        {
                            body.livingBody.SetActive(false);
                            body.deadBody.SetActive(true);
                            body.deadBody.transform.Rotate(new Vector3(0, 0, 1), 360 * Random.value);
                        }
                    }
                    else if (state is WalkingOutOfLevel walkOut)
                    {
                        SystemUpdate()
                            .Subscribe(_ =>
                            {
                                movement.Direction.Value = (walkOut.Target.position-component.transform.position).normalized;
                                movement.MaxSpeed = movement.Speed = component.normalSpeed;
                            })
                            .AddTo(walkOut);
                    }
                })
                .AddToLifecycleOf(component);
        }

        private void GoingToIdlePosition(GoingBackToIdle state, TouristBrainComponent tourist,
            MovementComponent movement)
        {
            SystemUpdate()
                .Select(_ => state.IdlePosition - (Vector2) tourist.transform.position)
                .Subscribe(delta =>
                {
                    if (delta.magnitude < 0.1f)
                    {
                        tourist.States.GoToState(new Idle(state.IdlePosition));
                    }
                    else
                    {
                        movement.MaxSpeed = movement.Speed = tourist.normalSpeed;
                        movement.Direction.Value = delta.normalized;
                    }
                })
                .AddTo(state);
        }

        private void Idle(Idle state, TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero;

            if (tourist.socialDistanceCollider)
            {
                //=== keep distance to each other ===
                Observable.Merge(
                        tourist.socialDistanceCollider.OnTriggerStayAsObservable()
                            .Where(c => c.gameObject.CompareTag("tourist"))
                            .Select(collider => (collider, move: true)),
                        tourist.socialDistanceCollider.OnTriggerExitAsObservable()
                            .Where(c => c.gameObject.CompareTag("tourist"))
                            .Select(collider => (collider, move: false)),
                        tourist.socialDistanceCollider.OnTriggerEnterAsObservable()
                            .Where(c => c.gameObject.CompareTag("tourist"))
                            .Select(collider => (collider, move: false))
                    )
                    .Select(x =>
                    {
                        if (x.move)
                        {
                            var delta = (Vector2) (tourist.transform.position - x.collider.transform.position);
                            if (delta == Vector2.zero) delta = Random.insideUnitCircle.normalized;
                            return delta;
                        }

                        return Vector2.zero;
                    })
                    .Subscribe(direction =>
                    {
                        movement.Direction.Value = direction;
                        movement.MaxSpeed = movement.Speed = tourist.idleSpeed;
                    })
                    .AddTo(state);

                //=== keep distance to border ===
                Observable.Merge(
                        tourist.socialDistanceCollider.OnTriggerStayAsObservable()
                            .Where(c => c.gameObject.CompareTag("levelBorder"))
                            .Select(collider => (collider, move: true)),
                        tourist.socialDistanceCollider.OnTriggerExitAsObservable()
                            .Where(c => c.gameObject.CompareTag("levelBorder"))
                            .Select(collider => (collider, move: false)),
                        tourist.socialDistanceCollider.OnTriggerEnterAsObservable()
                            .Where(c => c.gameObject.CompareTag("levelBorder"))
                            .Select(collider => (collider, move: false))
                    )
                    .Subscribe(x =>
                    {
                        var centerDelta = (state.IdlePosition - (Vector2) tourist.transform.position).normalized;
                        movement.Direction.Value = x.move ? centerDelta.normalized : Vector2.zero;
                        movement.MaxSpeed = movement.Speed = tourist.idleSpeed;
                    })
                    .AddTo(state);

                //=== start move randomly when not moved for 10 secs ===
                movement.Direction
                    .DistinctUntilChanged()
                    .Where(dir => dir == Vector2.zero)
                    .Throttle(TimeSpan.FromSeconds(Random.Range(1f,
                        tourist.idleMinTimeWithoutMovementInSeconds)))
                    .Do(_ => movement.Direction.Value =
                        (state.IdlePosition - (Vector2) movement
                            .transform.position).Rotate(Random.Range(-90, 90)))
                    .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(Random.value)))
                    .Subscribe(_ =>
                    {
                        movement.Direction.Value = Vector2.zero;
                        movement.MaxSpeed = movement.Speed = Random.Range(0f, tourist.idleSpeed);
                    })
                    .AddTo(state);
            }

            // TODO: talking to each other 
        }

        private void GoingToAttraction(GoingToAttraction attraction, TouristBrainComponent tourist,
            MovementComponent movement)
        {
            movement.MaxSpeed = movement.Speed = tourist.normalSpeed;

            SystemUpdate()
                .Select(_ => attraction.AttractionPosition - (Vector2) tourist.transform.position)
                .Do(delta => tourist.debugTargetDistance = delta)
                .Subscribe(delta =>
                {
                    if (delta.magnitude < 0.1f)
                    {
                        tourist.States.GoToState(new Interacting());
                    }
                    else
                    {
                        movement.Direction.Value =
                            (attraction.AttractionPosition - (Vector2) tourist.transform.position).normalized;
                    }
                })
                .AddTo(attraction);
        }

        private void Interacting(Interacting interacting, TouristBrainComponent tourist, MovementComponent movement)
        {
            movement.Direction.Value = Vector2.zero; // TODO: just stop movement here?
        }
    }
}