using SystemBase.StateMachineBase;
using Systems;
using GameState.Messages.Common;
using UniRx;

namespace GameState.States.Common
{
    [NextValidStates(typeof(Running))]
    public class Paused : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgUnpause>()
                .Subscribe(unpause => context.GoToState(new Running()))
                .AddTo(this);
        }
    }
}
