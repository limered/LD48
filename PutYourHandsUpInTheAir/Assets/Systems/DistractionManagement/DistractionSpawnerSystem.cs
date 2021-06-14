using System;
using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Distractions.States;
using UniRx;
using UnityEngine;
using Utils.Plugins;
using Object = UnityEngine.Object;

namespace Systems.DistractionManagement
{
    [GameSystem]
    public class DistractionSpawnerSystem : GameSystem<DistractionPrefabs, DistractionSpawnerComponent>
    {
        private readonly ReactiveProperty<DistractionPrefabs> _prefabs = new ReactiveProperty<DistractionPrefabs>();

        public override void Register(DistractionSpawnerComponent component)
        {
            component.GetComponent<MeshRenderer>().enabled = false;
            component.WaitPosition.GetComponent<MeshRenderer>().enabled = false;

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
                var prefab = _prefabs.Value.GetPrefab(component.DistractionType);
                var distraction = Object.Instantiate(prefab, component.transform.position, Quaternion.identity, component.transform);
                var distractionOrigin = distraction.GetComponent<DistractionOriginComponent>();
                distractionOrigin.StateContext = new StateContext<DistractionOriginComponent>(distractionOrigin);
                distractionOrigin.StateContext.Start(new DistractionStateWalkingIn(component));
            };
        }

        public override void Register(DistractionPrefabs component)
        {
            _prefabs.Value = component;
        }
    }
}
