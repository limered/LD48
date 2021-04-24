using SystemBase.StateMachineBase;
using Systems;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Common
{
    [NextValidStates(typeof(StartScreen))]
    public class GameOver : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgRestart>()
                .Subscribe(restart => context.GoToState(new StartScreen()))
                .AddTo(this);
        }
    }
}