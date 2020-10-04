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
        if(nextObject != null)
        {
            float distance = vehicle.Roundabout.GetAngleDistance(vehicle.GetLane(), vehicle.CurrentAngle, nextObject.CurrentAngle);
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

            var bounds = vehicle.GetBounds();

            if (vehicle.GetLane() < roundabout.lanes)
            {
                var hits = Physics.OverlapBox(bounds.center - transform.right * roundabout.laneWidth, bounds.extents * 2.5f, vehicle.bodyMesh.transform.rotation, ~0, QueryTriggerInteraction.Collide);
                canChangeLeft = true;
                foreach (var collider in hits)
                {
                    var otherVehicle = collider.GetComponentInParent<Vehicle>();
                    var obstacle = collider.GetComponentInParent<Obstacle>();
                    if ((otherVehicle && otherVehicle.GetLane() == vehicle.GetLane() && otherVehicle != vehicle)
                        || (obstacle && obstacle.GetLane() == vehicle.GetLane()))
                    {
                        canChangeLeft = false;
                    }
                }
            }
            if(vehicle.GetLane() > 1)
            {
                var hits = Physics.OverlapBox(bounds.center + transform.right * roundabout.laneWidth, bounds.extents * 2.5f, vehicle.bodyMesh.transform.rotation, ~0, QueryTriggerInteraction.Collide);
                canChangeRight = true;
                foreach (var collider in hits)
                {
                    var otherVehicle = collider.GetComponentInParent<Vehicle>();
                    var obstacle = collider.GetComponentInParent<Obstacle>();
                    if ((otherVehicle && otherVehicle.GetLane() == vehicle.GetLane() && otherVehicle != vehicle) 
                        || (obstacle && obstacle.GetLane() == vehicle.GetLane()))
                    {
                        canChangeRight = false;
                    }
                }
            }

            if (canChangeLeft || canChangeRight) {
                lastTurnTime = Time.time;
                followStartTime = Time.time;
            }

            if (canChangeLeft && !canChangeRight) vehicle.ChangeLaneLeft();
            else if (canChangeRight && !canChangeLeft) vehicle.ChangeLaneRight();
            else if(canChangeLeft && canChangeRight)
            {
                if (Random.value > 0.5f) vehicle.ChangeLaneLeft();
                else vehicle.ChangeLaneRight();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!vehicle) return;
        var bounds = vehicle.GetBounds();
        var roundabout = vehicle.Roundabout;
        Gizmos.color = Color.green;
        var prevMatrix = Gizmos.matrix;
        Gizmos.matrix = vehicle.bodyMesh.transform.localToWorldMatrix;
        Vector3 size = vehicle.bodyMesh.transform.InverseTransformVector(bounds.size) * 2.5f;

        Gizmos.DrawWireCube(vehicle.bodyMesh.transform.InverseTransformPoint(bounds.center + transform.right * roundabout.laneWidth), size);
        Gizmos.DrawWireCube(vehicle.bodyMesh.transform.InverseTransformPoint(bounds.center - transform.right * roundabout.laneWidth), size);
        Gizmos.matrix = prevMatrix;
    }
}
