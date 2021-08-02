using SystemBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Income
{
    public class PotentialIncomeComponent : GameComponent
    {
        public Text PotentialIncomeAmount;
        public GameObject VanishedIncome;

        public FloatReactiveProperty CurrentPotentialIncome = new FloatReactiveProperty(0);

    }
}