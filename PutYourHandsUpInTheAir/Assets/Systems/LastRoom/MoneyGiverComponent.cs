using SystemBase;

namespace Systems.LastRoom
{
    public class MoneyGiverComponent : GameComponent
    {
        public float Amount;
        public bool WantsToPay;
        public bool HasPaid;
    }
}