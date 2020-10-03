using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roundabout : MonoBehaviour
{
    public LineRenderer innerLaneMarkingPrefab;
    public LineRenderer outerLaneMarkingPrefab;
    public Vehicle trafficVehiclePrefab;

    public float laneWidth = 10.0f;
    public int lanes = 2;
    public float centerRadius = 1.0f;
    public float radiusSegmentRatio = 14.4f;
    public float radiusTrafficRatio = 0.4f;
    public int initialTraffic = 0;

    float maxRadius = 0.0f;

    void Awake()
    {
        maxRadius = centerRadius + laneWidth * lanes;
    }

    public void Setup()
    {
        for(int i = 0; i < lanes; i++)
        {
            int lane = i + 1;
            CreateLaneLines(lane, i != 0);
            float radius = GetLaneRadius(lane);
            int trafficCount = Mathf.Clamp((int)(radius * radiusTrafficRatio), 1, 50);
            float anglePerVehicle = 360.0f / trafficCount;
            for(int j = (i == lanes - 1 ? 1 : 0); j < trafficCount; j++)
            {
                var vehicle = Instantiate(trafficVehiclePrefab);
                vehicle.SetupForRoundabout(this);
                vehicle.gameObject.AddComponent<TrafficVehicleController>();
                vehicle.SetAngle(j * anglePerVehicle);
                vehicle.SetLane(lane);
                vehicle.maxSpeed = Random.Range(10.0f, 20.0f);
            }
        }

        CreateLaneLines(lanes + 1, false);
    }

    public float GetLaneRadius(float lane)
    {
        return centerRadius + laneWidth * lane - laneWidth * 0.5f;
    }

    public float GetLaneCirc(float lane)
    {
        return 2 * Mathf.PI * GetLaneRadius(lane);
    }

    public float MoveAngleAroundLane(float lane, float current, float distance)
    {
        float circ = GetLaneCirc(lane);
        float circAmount = distance / circ;
        float angleDelta = circAmount * 360.0f;

        return (current + angleDelta) % 360;
    }

    public Vector3 GetPointOnLane(float lane, float angle)
    {
        Vector3 dir = Vector3.forward;
        dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;

        return transform.position + dir * GetLaneRadius(lane);
    }

    void CreateLaneLines(int lane, bool isInner)
    {
        var lineRenderer = Instantiate(isInner ? innerLaneMarkingPrefab : outerLaneMarkingPrefab);
        var segmentCount = (int)(radiusSegmentRatio * GetLaneRadius(lane));
        var segmentAngle = 360.0f / segmentCount;
        lineRenderer.positionCount = segmentCount;
        Vector3[] positions = new Vector3[segmentCount];
        for(int i = 0; i  < segmentCount; i++)
        {
            positions[i] = GetPointOnLane(lane - 0.5f, segmentAngle * i) + Vector3.up * 0.05f;
        }
        lineRenderer.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;
        for (int i = 0; i < lanes; i++)
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, centerRadius + laneWidth * (i + 1) - (laneWidth * 0.5f));
        }

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, centerRadius);
#endif
    }
}
