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
    public float initialObstacles = 1;

    float maxRadius = 0.0f;

    List<ILaneUser>[] laneObjects;

    void Awake()
    {
        maxRadius = centerRadius + laneWidth * lanes;
        laneObjects = new List<ILaneUser>[lanes];
        for(int i = 0; i < lanes; i++)
        {
            laneObjects[i] = new List<ILaneUser>();
        }
    }

    public void Setup(Vehicle playerVehicle)
    {
        laneObjects[playerVehicle.GetLane() - 1].Add(playerVehicle);
        playerVehicle.OnLaneChanged.AddListener(HandleLaneChange);
        playerVehicle.OnDestroyed.AddListener(HandleDestroyed);

        int obstaclesSpawned = 0;
        int vehicleCount = 0;
        for(int i = 0; i < lanes; i++)
        {
            int lane = i + 1;
            CreateLaneLines(lane, i != 0);
            float radius = GetLaneRadius(lane);
            int trafficCount = Mathf.Clamp((int)(radius * radiusTrafficRatio), 1, 50);
            float anglePerVehicle = 360.0f / trafficCount;
            for(int j = (i == lanes - 1 ? 1 : 0); j < trafficCount; j++)
            {
                if(obstaclesSpawned < initialObstacles)
                {
                    var obstaclePrefab = ObstaclePrefabManager.GetRandomObstacle();
                    GameObject obstacleObject = Instantiate(obstaclePrefab);
                    var obstacle = obstacleObject.GetComponent<Obstacle>();
                    obstacle.CurrentLane = lane;
                    obstacle.CurrentAngle = j * anglePerVehicle;
                    obstacle.transform.position = GetPointOnLane(lane, obstacle.CurrentAngle);
                    laneObjects[i].Add(obstacle);
                    obstaclesSpawned += 1;
                } else
                {
                    var vehicle = Instantiate(trafficVehiclePrefab);
                    vehicle.id = ++vehicleCount;
                    vehicle.SetupForRoundabout(this);
                    vehicle.SetAngle(j * anglePerVehicle);
                    vehicle.SetLane(lane);
                    vehicle.maxSpeed = Random.Range(10.0f, 20.0f);
                    laneObjects[i].Add(vehicle);
                    vehicle.OnLaneChanged.AddListener(HandleLaneChange);
                    vehicle.OnDestroyed.AddListener(HandleDestroyed);
                }
            }
        }

        CreateLaneLines(lanes + 1, false);

        for(int i = 0; i < lanes; i++)
        {
            SortLane(i + 1);
        }
    }

    public float GetLaneRadius(float lane)
    {
        return centerRadius + laneWidth * lane - laneWidth * 0.5f;
    }

    public float GetLaneCirc(float lane)
    {
        return 2 * Mathf.PI * GetLaneRadius(lane);
    }

    public float GetAngleDistance(int lane, float from, float to)
    {
        float diff = to - from;
        if (diff < 0.0f) diff += 360.0f;
        return diff / 360 * GetLaneCirc(lane);
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

    void SortLane(int lane)
    {
        laneObjects[lane - 1].Sort((a, b) => a.CurrentAngle.CompareTo(b.CurrentAngle));
    }

    void HandleLaneChange(Vehicle vehicle, int from, int to)
    {
        laneObjects[from - 1].Remove(vehicle);
        laneObjects[to - 1].Add(vehicle);
        SortLane(to);
    }

    void HandleDestroyed(Vehicle vehicle)
    {
        laneObjects[vehicle.GetLane() - 1].Remove(vehicle);
    }

    public ILaneUser GetNextAhead(ILaneUser from)
    {
        int laneId = from.GetLane() - 1;
        var vehicles = laneObjects[laneId];
        int index = vehicles.FindIndex(v => v == from);
        int nextIndex = index + 1;
        if (nextIndex >= vehicles.Count) nextIndex = 0;
        if (vehicles[nextIndex] == from) return null;
        return vehicles[nextIndex];
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
