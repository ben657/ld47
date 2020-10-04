using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class PickupManager
{
    const string PickupsLabel = "PickupPrefabs";

    static List<GameObject> pickupPrefabs;

    public static bool loaded = false;

    public static void Init()
    {
        var operation = Addressables.LoadAssetsAsync<GameObject>(PickupsLabel, _ => { });
        operation.Completed += handle =>
        {
            pickupPrefabs = new List<GameObject>(handle.Result);
            loaded = true;
        };
    }

    public static GameObject GetPickup(string name)
    {
        return pickupPrefabs.Find(m => m.name == name);
    }

    public static GameObject GetRandomPickup()
    {
        return pickupPrefabs[Random.Range(0, pickupPrefabs.Count)];
    }
}
