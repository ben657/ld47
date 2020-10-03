using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class VehicleModelManager
{
    const string VehicleModelPrefabsLabel = "VehicleModelPrefabs";

    static List<GameObject> vehicleModels;

    public static bool loaded = false;

    public static void Init()
    {
        var operation = Addressables.LoadAssetsAsync<GameObject>(VehicleModelPrefabsLabel, _ => { });
        operation.Completed += handle =>
        {
            vehicleModels = new List<GameObject>(handle.Result);
            loaded = true;
        };
    }

    public static GameObject GetVehicleModel(string name)
    {
        return vehicleModels.Find(m => m.name == name);
    }

    public static GameObject GetRandomVehicleModel()
    {
        return vehicleModels[Random.Range(0, vehicleModels.Count)];
    }
}
