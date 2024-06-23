using SystemBase;
using Systems.Income;
using UnityEngine;

namespace Systems.GameMessages
{
    public class UIComponent : GameComponent
    {
        public AlertComponent Message;
        public PotentialIncomeComponent PotentialIncome;
        public GameObject EverybodyDiesMessage;
        public GameObject HelpMessage;
    }
}
