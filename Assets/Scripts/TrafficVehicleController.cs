using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class TrafficVehicleController : MonoBehaviour
{
    Vehicle vehicle;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        
    }
}
