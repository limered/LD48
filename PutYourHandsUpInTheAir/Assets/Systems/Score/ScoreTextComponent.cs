using SystemBase;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Score
{
    [RequireComponent(typeof(Text), typeof(ScoreComponent))]
    public class ScoreTextComponent : GameComponent
    {
        public float moneyPerSurvivor = 10f;
    }
}