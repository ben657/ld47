using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roundabout : MonoBehaviour
{
    public float maxRadius = 1.0f;
    public int lanes = 2;
    public float centerRadius = 1.0f;

    float laneWidth = 0.0f;

    void Awake()
    {
        laneWidth = (maxRadius - centerRadius) / (float)lanes;
    }

    public float GetLaneRadius(int lane)
    {
        return centerRadius + laneWidth * lane + laneWidth * 0.5f;
    }

    public float GetLaneCirc(int lane)
    {
        return 2 * Mathf.PI * GetLaneRadius(lane);
    }

    public float MoveAngleAroundLane(int lane, float current, float distance)
    {
        float circ = GetLaneCirc(lane);
        float circAmount = distance / circ;
        float angleDelta = circAmount * 360.0f;

        return current + angleDelta;
    }

    public Vector3 GetPointOnLane(int lane, float angle)
    {
        Vector3 dir = Vector3.forward;
        dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;

        return transform.position + dir * GetLaneRadius(lane);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;
        for(int i = 0; i < lanes; i++)
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, centerRadius + (((maxRadius - centerRadius) / lanes) * (i + 1)) + (laneWidth * 0.5f));
        }

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, centerRadius);
#endif
    }
}
