using SystemBase.StateMachineBase;
using Systems;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Common
{
    [NextValidStates(typeof(StartScreen))]
    public class Loading : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgFinishedLoading>()
                .Subscribe(loading => context.GoToState(new StartScreen()))
                .AddTo(this);
        }
    }
}