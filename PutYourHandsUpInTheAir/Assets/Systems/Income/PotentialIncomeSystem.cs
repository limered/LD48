using System.Collections.Generic;
using System.Linq;
using SystemBase;
using Systems.LastRoom;
using UniRx;
using UnityEngine;

namespace Systems.Income
{
    [GameSystem]
    public class PotentialIncomeSystem : GameSystem<PotentialIncomeMemoryComponent>
    {
        public override void Register(PotentialIncomeMemoryComponent component)
        {
            SystemUpdate(component)
                .Subscribe(CheckState)
                .AddTo(component);
        }

        private void CheckState(PotentialIncomeMemoryComponent obj)
        {
            IEnumerable<MoneyGiverComponent> tourists = FindAllTourists();
            obj.CurrentPotentialIncome.Value = tourists.Select(component => component.Amount).Sum();
        }

        private IEnumerable<MoneyGiverComponent> FindAllTourists()
        {
            return Object.FindObjectsOfType<MoneyGiverComponent>();
        }
    }
}
