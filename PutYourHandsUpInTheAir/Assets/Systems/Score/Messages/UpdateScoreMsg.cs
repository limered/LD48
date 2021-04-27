using Systems.Tourist;

namespace Systems.Score.Messages
{
    public class UpdateScoreMsg
    {
        public TouristDump[] Stats { get; }

        public UpdateScoreMsg(TouristDump[] finalStats)
        {
            Stats = finalStats;
        }
    }
}