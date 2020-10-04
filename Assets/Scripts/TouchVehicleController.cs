using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchVehicleController : MonoBehaviour, IPlayerController
{
    public float idleSpeed = 0.0f;
    public float minSpeed = 1.0f;

    Vector2 touchStart;
    public float deadzone = 1.0f;
    bool isVTouch = false;
    bool isHTouch = false;

    int lastTouchCount = 0;

    public Vehicle Vehicle { get; set; }

    void Awake()
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
        Vehicle.targetSpeed = idleSpeed;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (lastTouchCount <= 0 && Input.touchCount > 0)
                touchStart = touch.position;

            Vector2 diff = touch.position - touchStart;

            if (!isVTouch && !isHTouch)
            {
                Debug.Log(diff);
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y) && Mathf.Abs(diff.x) > deadzone)
                    isHTouch = true;
                if (Mathf.Abs(diff.x) <= Mathf.Abs(diff.y) && Mathf.Abs(diff.y) > deadzone)
                    isVTouch = true;
            }

            if(isVTouch)
            {
                if (diff.y > deadzone)
                    Vehicle.targetSpeed = Vehicle.maxSpeed;
                else if (diff.y < -deadzone)
                    Vehicle.targetSpeed = minSpeed;
            }

            if(touch.phase == TouchPhase.Ended && isHTouch)
            {
                if (diff.x > deadzone)
                    Vehicle.ChangeLaneRight();
                else if (diff.x < -deadzone)
                    Vehicle.ChangeLaneLeft();
            }
        }
        else
        {
            isVTouch = false;
            isHTouch = false;
        }

        lastTouchCount = Input.touchCount;
    }
}
