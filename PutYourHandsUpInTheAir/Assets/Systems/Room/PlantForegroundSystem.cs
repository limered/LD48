using SystemBase;
using UnityEngine;

[GameSystem]
public class PlantForegroundSystem : GameSystem<PlantForegroundComponent>
{
    public override void Register(PlantForegroundComponent component)
    {
        var plantObject = component.PlantPrefabs[(int)(Random.value * component.PlantPrefabs.Length)];
        GameObject.Instantiate(plantObject, component.transform);
    }
}
