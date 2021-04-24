using SystemBase;
using UniRx;

namespace Systems.Player
{
    public class PlayerComponent : GameComponent
    {
        public BoolReactiveProperty IsMoving;
    }
}