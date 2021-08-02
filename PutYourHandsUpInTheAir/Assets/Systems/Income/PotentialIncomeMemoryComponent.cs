using SystemBase;
using UniRx;

namespace Systems.Income
{
    public class PotentialIncomeMemoryComponent : GameComponent
    {
        public FloatReactiveProperty CurrentPotentialIncome = new FloatReactiveProperty(0);
    }
}