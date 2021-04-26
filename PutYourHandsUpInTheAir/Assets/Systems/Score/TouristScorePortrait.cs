using SystemBase;
using Systems.Tourist;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Score
{
    public class TouristScorePortrait : GameComponent
    {
        public TouristDump touristStats;
        public Image image;
        public GameObject deathBadge;
        public GameObject moneyBadge;
        public Text name;
    }
}