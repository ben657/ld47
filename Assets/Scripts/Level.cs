using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Roundabout roundabout;
    public Vehicle playerVehicle;

    // Start is called before the first frame update
    void Start()
    {
        playerVehicle.SetupForRoundabout(roundabout);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
