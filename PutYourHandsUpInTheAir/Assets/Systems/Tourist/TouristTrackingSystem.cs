using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.Room;
using Systems.Tourist.States;
using GameState.Messages;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = System.Object;
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

        public override void Register(RoomComponent component)
        {
            WaitOn<TouristConfigComponent>().Then(config =>
                    component.State.CurrentState
                        .First(state => state is RoomWalkIn)
                        .Do(_ => _tourists = GenerateTourists(config, component)))
                .Subscribe()
                .AddToLifecycleOf(component);
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
                        room.SpawnInPosition.transform.position + (Vector3) Random.insideUnitCircle,
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