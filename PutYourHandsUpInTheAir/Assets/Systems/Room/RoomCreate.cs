using SystemBase.StateMachineBase;

namespace Systems.Room
{
    [NextValidStates(typeof(RoomPrepare))]
    public class RoomCreate : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }

    [NextValidStates(typeof(RoomWalkIn))]
    public class RoomPrepare : BaseState<RoomComponent>
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

    public class RoomDestroy : BaseState<RoomComponent>
    {
        public override void Enter(StateContext<RoomComponent> context)
        {
        }
    }
}
