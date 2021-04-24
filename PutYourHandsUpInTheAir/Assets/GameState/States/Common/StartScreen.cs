using SystemBase.StateMachineBase;
using Systems;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Common
{
    [NextValidStates(typeof(Running))]
    public class StartScreen : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgStart>()
                .Subscribe(start => context.GoToState(new Running()))
                .AddTo(this);
        }
    }
}