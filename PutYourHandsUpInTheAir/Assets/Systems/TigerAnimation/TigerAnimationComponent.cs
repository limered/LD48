using System.Collections;
using System.Collections.Generic;
using SystemBase;
using UnityEngine;

public class TigerAnimationComponent : GameComponent
{
    public TigerState CurrentState;
}

public enum TigerState
{
    Sleeping,
    Awake,
    Walking,
    Kill
}
