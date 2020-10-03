using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    public float speed = 0.0f;

    Rigidbody body;

    Roundabout roundabout;
    int currentLane = 0;
    float currentAngle = 0.0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupForRoundabout(Roundabout roundabout)
    {
        this.roundabout = roundabout;
        currentLane = this.roundabout.lanes;
    }

    public Vector3 GetTangentToRoundabout()
    {
        return -Vector3.Cross((transform.position - roundabout.transform.position).normalized, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        currentAngle = roundabout.MoveAngleAroundLane(currentLane, currentAngle, speed * Time.deltaTime);
        transform.position = roundabout.GetPointOnLane(currentLane, currentAngle);
        transform.forward = GetTangentToRoundabout();
    }
}
