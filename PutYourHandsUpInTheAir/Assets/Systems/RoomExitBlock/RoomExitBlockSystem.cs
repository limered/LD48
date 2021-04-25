﻿using SystemBase;
using Systems.Room;
using UniRx;
using UnityEngine;

namespace Systems.RoomExitBlock
{
    public class RoomExitBlockSystem : GameSystem<RoomExitBlockComponent, RoomComponent>
    {
        public override void Register(RoomComponent component)
        {
            WaitOn<RoomExitBlockComponent>().Subscribe(roomExitBlockComponent =>
                {
                    var divider = 100f / roomExitBlockComponent.sprites.Length;
                    component.RoomTimeProgress.Subscribe(roomProgress =>
                        UpdateBlockedPath(roomProgress, divider, roomExitBlockComponent));
                    component.State.AfterStateChange.Where(newState => newState is RoomWalkOut)
                        .Subscribe(_ => RemoveRoomExitBlock(roomExitBlockComponent))
                        .AddTo(component);
                })
                .AddTo(component);
        }

        public override void Register(RoomExitBlockComponent component)
        {
            RegisterWaitable(component);
        }

        private static void RemoveRoomExitBlock(RoomExitBlockComponent roomExitBlockComponent)
        {
            Object.Destroy(roomExitBlockComponent.gameObject);
        }

        private static void UpdateBlockedPath(float roomProgress, float divider,
            RoomExitBlockComponent roomExitBlockComponent)
        {
            var progressIndex = (int) (roomProgress / divider);
            if (progressIndex >= roomExitBlockComponent.sprites.Length)
            {
                progressIndex = roomExitBlockComponent.sprites.Length - 1;
            }

            roomExitBlockComponent.spriteRenderer.sprite = roomExitBlockComponent.sprites[progressIndex];
        }
    }
}