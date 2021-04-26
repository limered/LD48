using System.Collections;
using System.Collections.Generic;
using SystemBase;
using UnityEngine;

[GameSystem]
public class BushTreesSystem : GameSystem<BushTreesComponent>
{
    public override void Register(BushTreesComponent component)
    {
        var plantObject = component.BushTreesPrefabs[(int)(Random.value * component.BushTreesPrefabs.Length)];
        GameObject.Instantiate(plantObject, component.transform);
    }
}
