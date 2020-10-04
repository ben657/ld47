using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, ILaneUser
{
    public float CurrentAngle { get; set; }
    public int CurrentLane { get; set; }

    public int GetLane()
    {
        return CurrentLane;
    }

    public float GetSpeed()
    {
        return 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
