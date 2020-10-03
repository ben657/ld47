using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class KeyboardVehicleController : MonoBehaviour
{
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
    }
}
