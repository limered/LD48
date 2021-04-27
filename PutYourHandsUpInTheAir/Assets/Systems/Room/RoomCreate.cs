using SystemBase.StateMachineBase;

namespace Systems.Room
{
    [NextValidStates(typeof(RoomWalkIn))]
    public class RoomCreate : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }

    [NextValidStates(typeof(RoomRunning))]
    public class RoomWalkIn : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }

    [NextValidStates(typeof(RoomWalkOut))]
    public class RoomRunning : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }

    [NextValidStates(typeof(RoomDestroy))]
    public class RoomWalkOut : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }
    [NextValidStates(typeof(RoomNext))]
    public class RoomDestroy : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }

    public class RoomNext : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }
}
