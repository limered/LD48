using SystemBase;
using UnityEngine;

public class BusComponent : GameComponent
{
    public Vector2 StartPosititon;
    public Vector2 EndPosititon;
    public bool enteredScene { get; set; }
    public bool leftScene { get; set; }
    public float DriveInTime;
}
