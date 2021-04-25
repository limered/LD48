using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase;
using GameState.Messages;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristTrackingSystem : GameSystem<TouristConfigComponent, TouristBrainComponent>
    {
        /// Assigned when GameMsgStart is received
        private GameObject[] _tourists;

        public override void Register(TouristConfigComponent config)
        {
            RegisterWaitable(config);

            MessageBroker.Default.Receive<GameMsgStart>()
                .Subscribe(start => { _tourists = GenerateTourists(config); }).AddToLifecycleOf(config);
            
            MessageBroker.Default.Publish(new GameMsgStart());
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

        private GameObject[] GenerateTourists(TouristConfigComponent config)
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
                    var objectInstance = UnityEngine.Object.Instantiate(config.touristPrefab, config.spawnPosition,
                        Quaternion.identity);

                    var brain = objectInstance.GetComponent<TouristBrainComponent>();
                    // var body = objectInstance.GetComponent<TouristBodyComponent>();
                    tourist.Apply(brain);

                    return objectInstance;
                })
                .ToArray();

            return tourists;
        }
    }
}