using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ObstaclePrefabManager
{
    const string ObstaclePrefabsLabel = "ObstaclePrefabs";

    static List<GameObject> obstaclePrefabs;

    public static bool loaded = false;

    public static void Init()
    {
        var operation = Addressables.LoadAssetsAsync<GameObject>(ObstaclePrefabsLabel, _ => { });
        operation.Completed += handle =>
        {
            obstaclePrefabs = new List<GameObject>(handle.Result);
            loaded = true;
        };
    }

    public static GameObject GetObstacle(string name)
    {
        return obstaclePrefabs.Find(m => m.name == name);
    }

    public static GameObject GetRandomObstacle()
    {
        return obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
    }
}
