using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

public class VehicleEvent : UnityEvent<Vehicle> { }
public class LaneChangeEvent : UnityEvent<Vehicle, int, int> { }

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    public float maxSpeed = 0.0f;
    public float acceleration = 0.0f;
    public float braking = 0.0f;
    public float throttle = 0.0f;
    public string meshName = null;
    public float laneChangeTime = 1.0f;
    public float followDistance = 1.0f;

    public VehicleEvent OnCollide = new VehicleEvent();
    public LaneChangeEvent OnLaneChanged = new LaneChangeEvent();

    Rigidbody body;
    MeshRenderer bodyMesh;

    Roundabout roundabout;
    public Roundabout Roundabout => roundabout;

    int currentLane = 0;
    int targetLane = 0;
    float laneChangeProgress = 0.0f;
    public bool IsChangingLane => currentLane != targetLane && laneChangeProgress < 1.0f;

    float currentAngle = 0.0f;
    public float CurrentAngle => currentAngle;

    float currentSpeed = 0.0f;
    public float targetSpeed = 0.0f;

    Vector3 lastPosition;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        lastPosition = transform.position;

        GameObject meshPrefab = meshName == null || meshName.Length == 0 ? VehicleModelManager.GetRandomVehicleModel() : VehicleModelManager.GetVehicleModel(meshName);
        var meshObject = Instantiate(meshPrefab);
        meshObject.transform.SetParent(transform, false);
        bodyMesh = meshObject.GetComponent<MeshRenderer>();
        bodyMesh.material.color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);

        targetSpeed = maxSpeed;
    }

    public void SetupForRoundabout(Roundabout roundabout)
    {
        this.roundabout = roundabout;
        currentLane = this.roundabout.lanes;
        targetLane = currentLane;
    }

    public Vector3 GetTangentToRoundabout()
    {
        return -Vector3.Cross((transform.position - roundabout.transform.position).normalized, Vector3.up);
    }

    public Vector3 GetVelocity()
    {
        return (transform.position - lastPosition) / Time.deltaTime;
    }

    public void ChangeLaneLeft()
    {
        if (IsChangingLane) return;

        targetLane = Mathf.Clamp(currentLane + 1, 1, roundabout.lanes);
        StartLaneChange();
    }

    public void ChangeLaneRight()
    {
        if (IsChangingLane) return;

        targetLane = Mathf.Clamp(currentLane - 1, 1, roundabout.lanes);
        StartLaneChange();
    }

    void StartLaneChange()
    {
        if (targetLane == currentLane) return;

        laneChangeProgress = 0.0f;
    }

    public void SetLane(int lane)
    {
        currentLane = lane;
        targetLane = lane;
    }

    public int GetLane()
    {
        if (!IsChangingLane) return currentLane;
        else return laneChangeProgress > 0.5f ? targetLane : currentLane;
    }

    public void SetAngle(float angle)
    {
        currentAngle = angle;
    }

    // Update is called once per frame
    void Update()
    {
        if (roundabout)
        {
            targetSpeed = maxSpeed;

            // Slow to match car in front
            Vehicle nextVehicle = roundabout.GetVehicleAhead(this);
            float distance = roundabout.GetAngleDistance(GetLane(), currentAngle, nextVehicle.CurrentAngle);
            if (distance < followDistance)
            {
                targetSpeed = nextVehicle.currentSpeed;
            }

            throttle = currentSpeed == targetSpeed ? 0.0f : currentSpeed < targetSpeed ? 1.0f : -1.0f;
            float changeRate = throttle < 0.0f && currentSpeed > 0.0f ? braking : acceleration;

            currentSpeed = Mathf.Clamp(currentSpeed + throttle * changeRate * Time.deltaTime, -maxSpeed * 0.5f, maxSpeed);

            currentAngle = roundabout.MoveAngleAroundLane(currentLane, currentAngle, currentSpeed * Time.deltaTime);
            transform.position = roundabout.GetPointOnLane(currentLane, currentAngle);

            // Lane changing lerp
            if (IsChangingLane)
            {
                if (laneChangeProgress <= 0.5f && laneChangeProgress + laneChangeTime * Time.deltaTime > 0.5f)
                    OnLaneChanged.Invoke(this, currentLane, targetLane);
                laneChangeProgress += laneChangeTime * Time.deltaTime;
                float laneLerpAmount = Mathf.SmoothStep(0.0f, 1.0f, laneChangeProgress);
                transform.position = Vector3.Lerp(transform.position, roundabout.GetPointOnLane(targetLane, currentAngle), laneLerpAmount);

                if (laneChangeProgress >= 1.0f)
                    currentLane = targetLane;
            }
        }

        transform.forward = GetVelocity();

        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.attachedRigidbody)
        {
            var vehicle = collision.collider.attachedRigidbody.GetComponent<Vehicle>();
            if (vehicle)
                OnCollide.Invoke(vehicle);

            Debug.Log("Collided");
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!UnityEditor.Selection.activeTransform) return;
        var v = UnityEditor.Selection.activeTransform.GetComponentInParent<Vehicle>();
        if (v && v == this)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
#endif
    }
}
