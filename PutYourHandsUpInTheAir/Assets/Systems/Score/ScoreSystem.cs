using System.Linq;
using SystemBase;
using Systems.Score.Messages;
using Systems.Tourist;
using GameState.Messages;
using UniRx;
using UnityEngine.UI;
using Utils.Plugins;

namespace Systems.Score
{
    [GameSystem]
    public class ScoreSystem : GameSystem<ScoreComponent, ScoreTextComponent>
    {
        private TouristDump[] _lastStats = new TouristDump[0];

        public override void Init()
        {
            base.Init();

            MessageBroker.Default.Receive<UpdateScoreMsg>()
                .Subscribe(msg => { _lastStats = msg.Stats; });

            MessageBroker.Default.Publish(new UpdateScoreMsg(new TouristDump[]
            {
                new TouristDump
                {
                    Name = "John Cena",
                    IsAlive = true,
                },
                new TouristDump
                {
                    Name = "Angela Merkel",
                    IsAlive = true,
                },
                new TouristDump
                {
                    Name = "Albert Einstein",
                    IsAlive = false,
                }
            }));
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
                        $"{survivors} of {survivors + causalities} tourists survived the trip. Nice job!\n\nYou earn {survivors * component.moneyPerSurvivor} Money.";
                })
                .AddToLifecycleOf(component);
        }
    }
}