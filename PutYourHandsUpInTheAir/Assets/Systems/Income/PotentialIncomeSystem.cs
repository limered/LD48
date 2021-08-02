using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.LastRoom;
using UniRx;
using UnityEngine;

namespace Systems.Income
{
    [GameSystem]
    public class PotentialIncomeSystem : GameSystem<PotentialIncomeComponent>
    {
        public override void Register(PotentialIncomeComponent component)
        {
            SystemUpdate(component)
                .Subscribe(CheckState)
                .AddTo(component);

            // TODO update UI
        }

        private void CheckState(PotentialIncomeComponent potentialIncome)
        {
            IEnumerable<MoneyGiverComponent> tourists = FindAllTourists();
            potentialIncome.CurrentPotentialIncome.Value = tourists.Select(component => component.Amount).Sum();
        }

        private IEnumerable<MoneyGiverComponent> FindAllTourists()
        {
            return Object.FindObjectsOfType<MoneyGiverComponent>();
        }
    }
}
