using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        PickupManager.Init();
        yield return new WaitUntil(() => 
            VehicleModelManager.loaded && 
            ObstaclePrefabManager.loaded &&
            PickupManager.loaded
        );

        var playerVehicle = Instantiate(playerVehiclePrefab);
        playerVehicle.SetupForRoundabout(roundabout);
        playerVehicle.SetAngle(0);
        playerVehicle.OnCollide.AddListener(v => playerDead = true);
        playerVehicle.OnDestroyed.AddListener(v => SceneManager.LoadScene(0));

        Camera.main.GetComponent<CameraController>().target = playerVehicle;

        roundabout.Setup(playerVehicle);

        setupComplete = true;
    }

    void UpdateScore()
    {
        int floored = Mathf.FloorToInt(score);
        if (floored > lastFlooredScore)
            ui.UpdateScore(Mathf.FloorToInt(score));

        lastFlooredScore = floored;
    }

    public void AddBonus(int amount)
    {
        score += amount;
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (!setupComplete || playerDead) return;

        score += Time.deltaTime * timeScoreMultiplier;
        UpdateScore();
    }
}
