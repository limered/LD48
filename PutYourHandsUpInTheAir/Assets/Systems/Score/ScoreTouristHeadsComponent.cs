using SystemBase;
using UnityEngine;

namespace Systems.Score
{
    [RequireComponent(typeof(RectTransform), typeof(ScoreComponent))]
    public class ScoreTouristHeadsComponent : GameComponent
    {
        /// must be same amount and order as defined in TouristConfigComponent
        public Sprite[] heads = new Sprite[0];

        public GameObject touristPortraitPrefab;
        public int touristsPerRow = 6;
        public float portraitRatingInterval = 1f;
        public float portraitRatingStartDelay = 2f;
    }
}