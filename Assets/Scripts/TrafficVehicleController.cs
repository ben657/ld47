using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class TrafficVehicleController : MonoBehaviour
{
    public float followDistance = 0.0f;

    Vehicle vehicle;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        var roundabout = vehicle.Roundabout;

        Vehicle nextVehicle = vehicle.Roundabout.GetVehicleAhead(vehicle);
        float distance = vehicle.Roundabout.GetAngleDistance(vehicle.GetLane(), vehicle.CurrentAngle, nextVehicle.CurrentAngle);
        if (distance < followDistance)
        {
            vehicle.targetSpeed = nextVehicle.CurrentSpeed;
        }
        else
        {
            vehicle.targetSpeed = vehicle.maxSpeed;
        }

        if(vehicle.targetSpeed < vehicle.maxSpeed)
        {
            bool canChangeLeft = false;
            bool canChangeRight = false;

            if (vehicle.GetLane() < roundabout.lanes)
            {
                canChangeLeft = !Physics.Raycast(transform.position, -transform.right, roundabout.laneWidth);
            }
            if(vehicle.GetLane() > 1)
            {
                canChangeRight = !Physics.Raycast(transform.position, -transform.right, roundabout.laneWidth);
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
}
