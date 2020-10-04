using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, ILaneUser
{
    public float CurrentAngle { get; set; }
    public int CurrentLane { get; set; }

    public Roundabout roundabout;

    public int GetLane()
    {
        return CurrentLane;
    }

    public float GetSpeed()
    {
        return 0.0f;
    }

    Vector3 GetForward()
    {
        Vector3 nextPos = roundabout.GetPointOnLane(CurrentLane, CurrentAngle + 1.0f);
        return (nextPos - transform.position).normalized;
    }

    public Vector3 GetFront()
    {
        var collider = GetComponent<SphereCollider>();
        return transform.position + GetForward() * collider.radius;
    }

    public Vector3 GetRear()
    {
        var collider = GetComponent<SphereCollider>();
        return transform.position - GetForward() * collider.radius;
    }
}
