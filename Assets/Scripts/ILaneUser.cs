using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaneUser
{
    float CurrentAngle { get; set; }
    int CurrentLane { get; set; }
    int GetLane();
    float GetSpeed();
}
