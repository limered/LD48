using System;
using System.Linq;
using SystemBase;
using Systems.Score.Messages;
using Systems.Tourist;
using GameState.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Plugins;
using Object = UnityEngine.Object;

namespace Systems.Score
{
    [GameSystem]
    public class ScoreSystem : GameSystem<ScoreComponent, ScoreTextComponent, ScoreTouristHeadsComponent>
    {
        private TouristDump[] _lastStats = new TouristDump[0];

        public override void Init()
        {
            base.Init();

            MessageBroker.Default.Receive<UpdateScoreMsg>()
                .Subscribe(msg => { _lastStats = msg.Stats; });
        }

        public override void Register(ScoreComponent component)
        {
            MessageBroker.Default.Receive<UpdateScoreMsg>()
                .Subscribe(msg => { component.touristStats.Value = msg.Stats; })
                .AddToLifecycleOf(component);

            component.touristStats.Value = _lastStats;
        }

        public override void Register(ScoreTextComponent component)
        {
            var score = component.GetComponent<ScoreComponent>();
            var text = component.GetComponent<Text>();

            score.touristStats
                .Where(x => x != null && x.Length > 0)
                .Subscribe(tourists =>
                {
                    var survivors = tourists.Count(x => x.IsAlive);
                    var causalities = tourists.Count(x => !x.IsAlive);

                    text.text =
                        $"{survivors} of {survivors + causalities} tourists survived the trip. You've earned {survivors * component.moneyPerSurvivor} Money.";
                    UnityEngine.Cursor.visible = true;
                })
                .AddToLifecycleOf(component);
        }


        public override void Register(ScoreTouristHeadsComponent component)
        {
            var score = component.GetComponent<ScoreComponent>();
            var rect = component.GetComponent<RectTransform>();
            var grid = component.GetComponent<GridLayoutGroup>();

            score.touristStats
                .Where(x => x != null && x.Length > 0)
                .Select(x => x.ToObservable())
                .SelectMany(x => x)
                .Select(tourist =>
                {
                    var portrait = Object.Instantiate(component.touristPortraitPrefab, component.transform);
                    var portraitConfig = portrait.GetComponent<TouristScorePortrait>();
                    portraitConfig.moneyBadge.SetActive(false);
                    portraitConfig.deathBadge.SetActive(false);
                    portraitConfig.image.sprite = component.heads[tourist.HeadPartIndex];
                    portraitConfig.name.text = tourist.Name;

                    return (tourist, portraitConfig);
                })
                .Delay(TimeSpan.FromSeconds(component.portraitRatingStartDelay))
                .WithInterval(TimeSpan.FromSeconds(component.portraitRatingInterval))
                .Subscribe(x =>
                {
                    x.portraitConfig.moneyBadge.SetActive(x.tourist.IsAlive);
                    x.portraitConfig.deathBadge.SetActive(!x.tourist.IsAlive);
                })
                .AddToLifecycleOf(component);
        }
    }
}