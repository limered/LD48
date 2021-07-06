using System;
using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Distractions.Messages;
using Systems.Distractions.States;
using Systems.Room.Events;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Systems.DistractionManagement
{
    [GameSystem]
    public class DistractionSpawnerSystem : GameSystem<DistractionPrefabs, DistractionSpawnerComponent>
    {
        private readonly ReactiveProperty<DistractionPrefabs> _prefabs = new ReactiveProperty<DistractionPrefabs>();

        public override void Register(DistractionSpawnerComponent component)
        {
            component.GetComponent<MeshRenderer>().enabled = false;
            component.WaitArea.GetComponent<MeshRenderer>().enabled = false;

            _prefabs.WhereNotNull()
                .Subscribe(_ => Observable.Interval(TimeSpan.FromSeconds(component.TimeBetweenSpawnsInSec))
                    .Subscribe(SpawnDistraction(component))
                    .AddTo(component))
                .AddTo(component);
        }

        private Action<long> SpawnDistraction(DistractionSpawnerComponent component)
        {
            return _ =>
            {
                if (component.CurrentDistraction)
                {
                    return;
                }

                component.WaitPosition = CalculatePositionFromArea(component.WaitArea);
                var distractionOrigin = CreateDistractionOrigin(component);
                ConfigureDistractionOrigin(component, distractionOrigin);

                component.CurrentDistraction = distractionOrigin;
            };
        }

        private static void ConfigureDistractionOrigin(DistractionSpawnerComponent component,
            DistractionOriginComponent distractionOrigin)
        {
            distractionOrigin.StateContext = new StateContext<DistractionOriginComponent>(distractionOrigin);
            distractionOrigin.StateContext.Start(new DistractionStateWalkingIn(component));
            distractionOrigin.StateContext.CurrentState
                .Where(state => state is DistractionStateAborted)
                .Subscribe(_ => Observable
                    .Timer(TimeSpan.FromSeconds(component.WaitTimebeforeRespawn))
                    .Subscribe(_ => { component.CurrentDistraction = null; }))
                .AddTo(distractionOrigin);
        }

        private DistractionOriginComponent CreateDistractionOrigin(DistractionSpawnerComponent component)
        {
            var prefab = _prefabs.Value.GetPrefab(component.DistractionType);
            var distraction =
                Object.Instantiate(prefab, component.transform.position, Quaternion.identity, component.transform);
            var distractionOrigin = distraction.GetComponent<DistractionOriginComponent>();

            MessageBroker.Default.Receive<RoomTimerEndedAction>()
                .Subscribe(_msg => MessageBroker.Default.Publish(new AbortDistractionAction(distractionOrigin)))
                .AddTo(distraction);

            return distractionOrigin;
        }

        private Vector3 CalculatePositionFromArea(GameObject waitArea)
        {
            var meshFilter = waitArea.GetComponent<MeshFilter>();
            var min = meshFilter.mesh.bounds.min + waitArea.transform.position;
            var max = meshFilter.mesh.bounds.max + waitArea.transform.position;

            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0.0f);
        }

        public override void Register(DistractionPrefabs component)
        {
            _prefabs.Value = component;
        }
    }
}
