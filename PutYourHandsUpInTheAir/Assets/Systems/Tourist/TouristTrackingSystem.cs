using System.Linq;
using SystemBase;
using SystemBase.StateMachineBase;
using Systems.GameMessages.Messages;
using Systems.Room;
using Systems.Room.Events;
using Systems.Score.Messages;
using Systems.Tourist.States;
using GameState.Messages;
using GameState.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using Utils.Plugins;
using Random = UnityEngine.Random;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristTrackingSystem : GameSystem<TouristConfigComponent, TouristBrainComponent, RoomComponent>
    {
        /// Assigned when Room switches to RoomWalkIn state
        /// temporary variable holding all tourists that started alive in the current Room 
        private GameObject[] _tourists;

        /// Created when Game starts, and the first Room is entered
        /// Elements get populated when Tourists leave the room or die
        /// This variable is present during the whole game and can always be used
        /// to check for the current survival status of the group
        private TouristDump[] _touristDumps;

        public TouristDump[] TouristStats => _touristDumps.ToArray();

        public override void Register(TouristConfigComponent config)
        {
            RegisterWaitable(config);

            //=== Reset tourists ====
            IoC.Game.GameStateContext.CurrentState
                .Where(state => state is Running)
                .Subscribe(_ =>
                {
                    Debug.Log("------------------ NEW GAME ----------------");
                    _tourists = null;
                    _touristDumps = null;
                })
                .AddToLifecycleOf(config);
        }

        public override void Register(RoomComponent room)
        {
            Debug.Log($"NEW ROOM {room.name}");

            WaitOn<TouristConfigComponent>()
                .Then(config =>
                    room.State.CurrentState
                        .First(state => state is RoomWalkIn)
                        .Do(_ =>
                        {
                            if (_tourists == null) //first level
                            {
                                Debug.Log("RoomWalkIn - first level");
                                _tourists = GenerateTourists(config, room);
                                _touristDumps = _tourists
                                    .Select(t => new TouristDump(t.GetComponent<TouristBrainComponent>())).ToArray();
                                
                                MessageBroker.Default.Publish(new ShowInitialPotentialIncome
                                {
                                    InitialPotentialIncome = _touristDumps.Length * 100
                                });
                            }
                            else //level 2 -> END
                            {
                                Debug.Log("RoomWalkIn - 2-END level");
                                _tourists = LoadTourists(config, room);
                            }
                        }))
                .Subscribe()
                .AddToLifecycleOf(room);

            WaitOnAllTouristsInIdleToStart(room);

            WaitOnRoomToGetInWalkOutState(room);
        }

        private void WaitOnRoomToGetInWalkOutState(RoomComponent room)
        {
            room.State.CurrentState
                .Where(state => state is RoomWalkOut)
                .Subscribe(state => PutAllTouristsInWalkOutState(room, state))
                .AddToLifecycleOf(room);
        }

        private void PutAllTouristsInWalkOutState(RoomComponent room, BaseState<RoomComponent> state)
        {
            foreach (var tourist in _tourists.Where(t => t != null))
            {
                tourist.GetComponent<TouristBrainComponent>()
                    .States
                    .GoToState(new WalkingOutOfLevel(room.SpawnOutPosition.transform));
            }

            //=== Collect walked out Tourists ====
            room.SpawnOutPosition
                .OnTriggerEnterAsObservable()
                .Where(c => c.gameObject.CompareTag("tourist"))
                .SelectWhereNotNull(c => c.gameObject.GetComponent<TouristBrainComponent>())
                .Subscribe(brain =>
                {
                    brain.States.GoToState(new WalkedOut());
                    
                    for (int i = 0; i < _tourists.Length; i++)
                    {
                        if (_tourists[i])
                        {
                            var tourist = _tourists[i].GetComponent<TouristBrainComponent>();
                            if (brain == tourist)
                            {
                                _touristDumps[i] = new TouristDump(brain);
                            }
                        }
                    }

                    CheckForRoomFinish();
                })
                .AddTo(state);
        }

        private void CheckForRoomFinish()
        {
            if (_tourists.Where(x => x != null)
                .Select(x => x.GetComponent<TouristBrainComponent>().States.CurrentState.Value)
                .All(state => state is WalkedOut || state is Dead)
            )
            {
                if (_touristDumps.Any(x => x.IsAlive))
                {
                    Debug.Log($"Room finished. {_touristDumps.Count(x => x.IsAlive)} tourists survived");
                    MessageBroker.Default.Publish(new RoomAllTouristsLeft());
                }
                else
                {
                    Debug.Log($"Room finished. Everyone died...");
                    MessageBroker.Default.Publish(new RoomEverybodyDied());
                }
                
                MessageBroker.Default.Publish(new UpdateScoreMsg(TouristStats));
            }
        }

        public override void Register(TouristBrainComponent component)
        {
            var body = component.GetComponent<TouristBodyComponent>();

            //=== Collect dead Tourists ====
            component.States.CurrentState.Where(state => state is Dead)
                .Subscribe(_ =>
                {
                    for (int i = 0; i < _tourists.Length; i++)
                    {
                        if (_tourists[i])
                        {
                            var tourist = _tourists[i].GetComponent<TouristBrainComponent>();
                            if (component == tourist)
                            {
                                _touristDumps[i] = new TouristDump(component);
                            }
                        }
                    }

                    CheckForRoomFinish();
                })
                .AddToLifecycleOf(component);

            //=== Update name ====
            component.touristName
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Subscribe(name => component.gameObject.name = name)
                .AddToLifecycleOf(component);

            //=== Update Head/Body Sprite ====
            WaitOn<TouristConfigComponent>()
                .Then(config => Observable.Zip(component.headPartIndex, component.bodyPartIndex,
                        (headIndex, bodyIndex) => (headIndex, bodyIndex))
                    .Do(i =>
                    {
                        if (i.headIndex <= config.topParts.Length)
                            body.head.sprite = config.topParts[i.headIndex];
                        if (i.bodyIndex <= config.bottomParts.Length)
                            body.body.sprite = config.bottomParts[i.bodyIndex];
                    }))
                .Subscribe()
                .AddToLifecycleOf(component);
        }

        private static bool RoomIsInWalkInState(RoomComponent component)
        {
            return component.State.CurrentState.Value is RoomWalkIn;
        }

        private void WaitOnAllTouristsInIdleToStart(RoomComponent room)
        {
            SystemUpdate()
                .Where(_ => RoomIsInWalkInState(room))
                .Where(_ => AllTouristsAreInIdle())
                .First()
                .Subscribe(_ => StartRoom())
                .AddToLifecycleOf(room);
        }

        private bool AllTouristsAreInIdle()
        {
            return _tourists != null &&
                   _tourists.Where(x => x != null)
                       .All(t => t.GetComponent<TouristBrainComponent>().States.CurrentState.Value is Idle);
        }

        private void StartRoom()
        {
            MessageBroker.Default.Publish(new RoomAllTouristsEntered());
        }

        private GameObject[] GenerateTourists(TouristConfigComponent config, RoomComponent room)
        {
            var tourists = Enumerable.Range(0, config.initialTouristCount)
                .Select(_ => new TouristDump
                {
                    Name = TouristNames.All[Random.Range(0, TouristNames.All.Length)],
                    IsAlive = true,
                    SocialDistancingRadius = Random.Range(0.1f, 1f),
                    HeadPartIndex = Random.Range(0, config.topParts.Length),
                    BodyPartIndex = Random.Range(0, config.bottomParts.Length),
                })
                .Select(tourist =>
                {
                    var objectInstance = Object.Instantiate(config.touristPrefab,
                        room.SpawnInPosition.transform.position + (Vector3) Random.insideUnitCircle,
                        Quaternion.identity, room.TouristGroup != null ? room.TouristGroup.transform : room.transform);

                    var brain = objectInstance.GetComponent<TouristBrainComponent>();
                    brain.tag = "tourist";
                    tourist.Apply(brain);
                    brain.States.Start(new GoingIntoLevel(brain));

                    return objectInstance;
                })
                .ToArray();

            return tourists;
        }

        private GameObject[] LoadTourists(TouristConfigComponent config, RoomComponent room)
        {
            return _touristDumps.Select(tourist =>
            {
                if (!tourist.IsAlive) return null;

                var objectInstance = Object.Instantiate(config.touristPrefab,
                    room.SpawnInPosition.transform.position + (Vector3) Random.insideUnitCircle,
                    Quaternion.identity, room.TouristGroup != null ? room.TouristGroup.transform : room.transform);

                var brain = objectInstance.GetComponent<TouristBrainComponent>();
                brain.tag = "tourist";
                tourist.Apply(brain);
                //brain.States.Start(new GoingIntoLevel());

                return objectInstance;
            }).ToArray();
        }
    }
}