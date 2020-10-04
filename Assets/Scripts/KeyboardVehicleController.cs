using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class KeyboardVehicleController : MonoBehaviour, IPlayerController
{
    public float idleSpeed = 0.0f;
    public float minSpeed = 1.0f;

    public Vehicle Vehicle { get; set; }

    private void Awake()
    {
        Vehicle = GetComponent<Vehicle>();
        Vehicle.OnCollide.AddListener(u =>
        {
            Vehicle.StartDestroy();
            if (u is Vehicle) ((Vehicle)u).StartDestroy();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Vehicle.ChangeLaneLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) Vehicle.ChangeLaneRight();

        if (Input.GetKey(KeyCode.DownArrow)) Vehicle.targetSpeed = minSpeed;
        else if (Input.GetKey(KeyCode.UpArrow)) Vehicle.targetSpeed = Vehicle.maxSpeed;
        else Vehicle.targetSpeed = Vehicle.maxSpeed * 0.75f;
    }
}
