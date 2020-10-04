using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class TrafficVehicleController : MonoBehaviour
{
    public float followDistance = 0.0f;
    public float arseDistance = 0.0f;
    public float backoffSpeed = 0.0f;
    public float minFollowTime = 0.0f;
    public float turnDelay = 0.0f;
    public float minMergeDist = 2.0f;

    Vehicle vehicle;
    float followStartTime = 0.0f;
    float lastTurnTime = 0.0f;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    void ResetFollowing()
    {
        vehicle.targetSpeed = vehicle.maxSpeed;
        followStartTime = 0.0f;
    }

    private void FixedUpdate()
    {
        var roundabout = vehicle.Roundabout;

        // Follow the next object until we can go around them
        ILaneUser nextObject = vehicle.Roundabout.GetNextAhead(vehicle);
        if (nextObject != null)
        {
            float distance = vehicle.Roundabout.GetAngleDistance(vehicle.GetLane(), vehicle.GetFront(), nextObject.GetRear());
            if (distance < followDistance)
            {
                vehicle.targetSpeed = distance < arseDistance ? nextObject.GetSpeed() - backoffSpeed : nextObject.GetSpeed();
                if (followStartTime == 0.0f) followStartTime = Time.time;
            }
            else if (distance > followDistance + 0.2f)
            {
                ResetFollowing();
            }
        }
        else
        {
            ResetFollowing();
        }

        float targetFollowTime = nextObject != null && nextObject is Obstacle ? 0.0f : minFollowTime;
        // If we've been following for a few seconds, try and go around the object
        if (vehicle.targetSpeed < vehicle.maxSpeed && Time.time - followStartTime >= targetFollowTime && Time.time - lastTurnTime > turnDelay)
        {
            bool canChangeLeft = false;
            bool canChangeRight = false;


            if (vehicle.GetLane() < roundabout.lanes)
            {
                canChangeLeft = true;

                int targetLane = vehicle.GetLane() + 1;
                ILaneUser ahead = roundabout.GetNextAhead(targetLane, vehicle);
                ILaneUser behind = roundabout.GetNextBehind(targetLane, vehicle);
                Vector3 offset = -transform.right * roundabout.laneWidth;

                if (ahead != null && Vector3.Dot(transform.forward, ahead.GetRear() - vehicle.GetFront()) < 0.0f) canChangeLeft = false;
                if (behind != null && Vector3.Dot(-transform.forward, behind.GetFront() - vehicle.GetRear()) < 0.0f) canChangeLeft = false;

                if (canChangeLeft)
                {
                    if (ahead != null)
                    {
                        float frontDist2 = (ahead.GetRear() - (vehicle.GetFront() + offset)).sqrMagnitude;
                        if (frontDist2 < minMergeDist * minMergeDist) canChangeLeft = false;
                    }
                    if (behind != null)
                    {
                        float rearDist2 = ((vehicle.GetRear() + offset) - behind.GetFront()).sqrMagnitude;
                        if (rearDist2 < minMergeDist * minMergeDist) canChangeLeft = false;
                    }
                }
            }
            if (vehicle.GetLane() > 1)
            {
                canChangeRight = true;

                int targetLane = vehicle.GetLane() - 1;
                ILaneUser ahead = roundabout.GetNextAhead(targetLane, vehicle);
                ILaneUser behind = roundabout.GetNextBehind(targetLane, vehicle);

                if (ahead != null && Vector3.Dot(transform.forward, ahead.GetRear() - vehicle.GetFront()) < 0.0f) canChangeRight = false;
                if (behind != null && Vector3.Dot(-transform.forward, behind.GetFront() - vehicle.GetRear()) < 0.0f) canChangeRight = false;

                if (canChangeRight)
                {
                    Vector3 offset = transform.right * roundabout.laneWidth;
                    if (ahead != null)
                    {
                        float frontDist2 = (ahead.GetRear() - (vehicle.GetFront() + offset)).sqrMagnitude;
                        if (frontDist2 < minMergeDist * minMergeDist) canChangeRight = false;
                    }
                    if(behind != null)
                    {
                        float rearDist2 = ((vehicle.GetRear() + offset) - behind.GetFront()).sqrMagnitude;
                        if (rearDist2 < minMergeDist * minMergeDist) canChangeRight = false;
                    }
                }
            }

            if (canChangeLeft || canChangeRight)
            {
                lastTurnTime = Time.time;
                followStartTime = Time.time;
            }

            if (canChangeLeft && !canChangeRight) vehicle.ChangeLaneLeft();
            else if (canChangeRight && !canChangeLeft) vehicle.ChangeLaneRight();
            else if (canChangeLeft && canChangeRight)
            {
                if (Random.value > 0.5f) vehicle.ChangeLaneLeft();
                else vehicle.ChangeLaneRight();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 offset = -transform.right * vehicle.Roundabout.laneWidth;
        Vector3 r = (vehicle.GetRear() - transform.forward * minMergeDist + offset);
        Vector3 f = (vehicle.GetFront() + transform.forward * minMergeDist + offset);
        Gizmos.DrawWireSphere(r, 0.25f);
        Gizmos.DrawWireSphere(f, 0.25f);
        Gizmos.DrawLine(f, r);

        offset *= -1.0f;
        r = (vehicle.GetRear() - transform.forward * minMergeDist + offset);
        f = (vehicle.GetFront() + transform.forward * minMergeDist + offset);
        Gizmos.DrawWireSphere(r, 0.25f);
        Gizmos.DrawWireSphere(f, 0.25f);
        Gizmos.DrawLine(f, r);

        ILaneUser infront = vehicle.Roundabout.GetNextAhead(vehicle.GetLane(), vehicle);
        if (infront != null)
            Gizmos.DrawWireSphere(((MonoBehaviour)infront).transform.position, 1.0f);
    }
}
