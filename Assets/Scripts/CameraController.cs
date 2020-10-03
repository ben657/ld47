using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vehicle target;
    public float distance = 10.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target) return;

        transform.position = target.transform.position - transform.forward * distance;
    }
}
