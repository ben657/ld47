using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Roundabout roundabout;
    public Vehicle playerVehiclePrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        VehicleModelManager.Init();
        yield return new WaitUntil(() => VehicleModelManager.loaded);

        var playerVehicle = Instantiate(playerVehiclePrefab);
        playerVehicle.SetupForRoundabout(roundabout);

        Camera.main.GetComponent<CameraController>().target = playerVehicle;

        roundabout.Setup(playerVehicle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
