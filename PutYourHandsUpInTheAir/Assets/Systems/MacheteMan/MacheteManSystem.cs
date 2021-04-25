using SystemBase;
using Systems.Movement;
using Systems.Room;
using Systems.RoomExitBlock;
using UniRx;
using UnityEngine;
using Utils.Plugins;

namespace Systems.MacheteMan
{
    [GameSystem]
    public class MacheteManSystem : GameSystem<RoomExitBlockComponent, RoomComponent, MacheteManComponent>
    {
        public override void Register(RoomComponent component)
        {
            RegisterWaitable(component);
        }

        public override void Register(MacheteManComponent component)
        {
            component.animator = component.GetComponent<Animator>();
            component.State.Start(new MacheteManCreate());
            WaitOn<RoomComponent>()
                .Subscribe(room => ResetMacheteMan(component, room))
                .AddToLifecycleOf(component);

            component.State.CurrentState.Where(_ => component.State.CurrentState.Value is MacheteManClearing)
                .Subscribe(_ => { component.animator.Play("Chopping"); }).AddTo(component);

            WaitOn<RoomExitBlockComponent>().Subscribe(roomBlockExitComponent =>
                {
                    component.TargetPosition =
                        new Vector2(
                            Random.Range(roomBlockExitComponent.leftBorder.position.x,
                                roomBlockExitComponent.rightBorder.position.x),
                            roomBlockExitComponent.leftBorder.position.y);

                    SystemUpdate().Where(_ => component.State.CurrentState.Value is MacheteManPrepare)
                        .Subscribe(_ => MoveToExitBlock(component,
                            component.GetComponent<MovementComponent>())).AddTo(component);
                })
                .AddTo(component);
        }

        public override void Register(RoomExitBlockComponent component)
        {
            RegisterWaitable(component);
        }

        private static void MoveToExitBlock(MacheteManComponent macheteManComponent,
            MovementComponent movementComponent)
        {
            var direction = macheteManComponent.TargetPosition - (Vector2) movementComponent.transform.position;
            if (direction.sqrMagnitude < 0.01)
            {
                movementComponent.Direction.Value = Vector2.zero;
                macheteManComponent.State.GoToState(new MacheteManClearing());
                return;
            }

            movementComponent.Direction.Value = direction.normalized;
        }

        private static void ResetMacheteMan(MacheteManComponent component, RoomComponent room)
        {
            component.State.GoToState(new MacheteManPrepare());
            component.transform.position = room.SpawnInPosition.transform.position;
        }
    }
}