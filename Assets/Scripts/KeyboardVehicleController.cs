using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class KeyboardVehicleController : MonoBehaviour
{
    public float minSpeed = 1.0f;

    Vehicle vehicle;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) vehicle.ChangeLaneLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) vehicle.ChangeLaneRight();

        if (Input.GetKey(KeyCode.DownArrow)) vehicle.targetSpeed = minSpeed;
        else if (Input.GetKey(KeyCode.UpArrow)) vehicle.targetSpeed = vehicle.maxSpeed;
        else vehicle.targetSpeed = vehicle.maxSpeed * 0.75f;
    }
}
