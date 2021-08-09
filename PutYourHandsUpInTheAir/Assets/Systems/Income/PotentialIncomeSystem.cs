using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SystemBase;
using Systems.GameMessages.Messages;
using Systems.LastRoom;
using Systems.Tourist;
using Systems.Tourist.States;
using UniRx;
using UnityEngine;

namespace Systems.Income
{
    [GameSystem]
    public class PotentialIncomeSystem : GameSystem<PotentialIncomeComponent>
    {
        public override void Register(PotentialIncomeComponent component)
        {
            component.CurrentPotentialIncome
                .Pairwise()
                .Select(pair => (pair.Previous, pair.Current, component))
                .Subscribe(IncomeUpdated)
                .AddTo(component);

            SystemUpdate(component)
                .Subscribe(CheckState)
                .AddTo(component);
        }

        private void IncomeUpdated((float last, float current, PotentialIncomeComponent component) data)
        {
            
            var (last, current, component) = data;
            var diff = current - last;
            component.PotentialIncomeAmount.text =
                component.CurrentPotentialIncome.Value.ToString(CultureInfo.InvariantCulture);

            MessageBroker.Default.Publish(new ReducePotentialIncomeAction
            {
                IncomeVanished = diff,
            });
        }

        private void CheckState(PotentialIncomeComponent potentialIncome)
        {
            IEnumerable<MoneyGiverComponent> tourists = FindAllTourists();
            potentialIncome.CurrentPotentialIncome.Value = tourists.Where(component =>
                    !(component.GetComponent<TouristBrainComponent>().StateContext.CurrentState.Value is Dead))
                .Select(component => component.Amount)
                .Sum();
        }

        private IEnumerable<MoneyGiverComponent> FindAllTourists()
        {
            return Object.FindObjectsOfType<MoneyGiverComponent>();
        }
    }
}
