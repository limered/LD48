using System.Linq;
using SystemBase;
using Systems.Room;
using Systems.Room.Events;
using Systems.Tourist.States;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Random = UnityEngine.Random;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristTrackingSystem : GameSystem<TouristConfigComponent, TouristBrainComponent, RoomComponent>
    {
        /// Assigned when GameMsgStart is received
        private GameObject[] _tourists;

        public override void Register(TouristConfigComponent config)
        {
            RegisterWaitable(config);
        }

        public override void Register(RoomComponent room)
        {
            WaitOn<TouristConfigComponent>()
                .Then(config =>
                    room.State.CurrentState
                        .First(state => state is RoomWalkIn)
                        .Do(_ => _tourists = GenerateTourists(config, room)))
                .Subscribe()
                .AddToLifecycleOf(room);

            WaitOnAllTouristsInIdleToStart(room);

            WaitOnRoomToGetInWalkOutState(room);
        }

        private void WaitOnRoomToGetInWalkOutState(RoomComponent room)
        {
            room.State.CurrentState
                .Where(state => state is RoomWalkOut)
                .Subscribe(_ => PutAllTouristsInWalkOutState(room))
                .AddToLifecycleOf(room);
        }

        private void PutAllTouristsInWalkOutState(RoomComponent room)
        {
            foreach (var tourist in _tourists)
            {
                tourist.GetComponent<TouristBrainComponent>()
                    .States
                    .GoToState(new WalkingOutOfLevel(room.SpawnOutPosition.transform));
            }
        }

        public override void Register(TouristBrainComponent component)
        {
            var body = component.GetComponent<TouristBodyComponent>();

            component.touristName
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Subscribe(name => component.gameObject.name = name)
                .AddToLifecycleOf(component);

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
            return _tourists != null && _tourists.All(t => t.GetComponent<TouristBrainComponent>().States.CurrentState.Value is Idle);
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
                    var objectInstance = UnityEngine.Object.Instantiate(config.touristPrefab,
                        room.SpawnInPosition.transform.position + (Vector3)Random.insideUnitCircle,
                        Quaternion.identity);

                    var brain = objectInstance.GetComponent<TouristBrainComponent>();
                    brain.tag = "tourist";
                    brain.States.Start(new GoingIntoLevel());

                    tourist.Apply(brain);

                    return objectInstance;
                })
                .ToArray();

            return tourists;
        }
    }
}