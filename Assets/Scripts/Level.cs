using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Roundabout roundabout;
    public Vehicle playerVehiclePrefab;
    public UIController ui;
    public float timeScoreMultiplier = 1.0f;

    float score = 0;
    int lastFlooredScore = 0;
    bool setupComplete = false;
    bool playerDead = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        VehicleModelManager.Init();
        ObstaclePrefabManager.Init();
        yield return new WaitUntil(() => VehicleModelManager.loaded && ObstaclePrefabManager.loaded);

        var playerVehicle = Instantiate(playerVehiclePrefab);
        playerVehicle.SetupForRoundabout(roundabout);
        playerVehicle.SetAngle(90);
        playerVehicle.OnCollide.AddListener(v => playerDead = true);

        Camera.main.GetComponent<CameraController>().target = playerVehicle;

        roundabout.Setup(playerVehicle);

        setupComplete = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!setupComplete || playerDead) return;

        score += Time.deltaTime * timeScoreMultiplier;

        int floored = Mathf.FloorToInt(score);
        if(floored > lastFlooredScore)
            ui.UpdateScore(Mathf.FloorToInt(score));

        lastFlooredScore = floored;
    }
}
