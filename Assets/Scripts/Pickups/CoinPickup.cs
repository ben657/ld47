using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : Pickup
{
    public int value = 0;
    public float spinSpeed = 1.0f;
    public float floatSpeed = 1.0f;

    float floatS = 0.0f;
    float baseHeight = 0.0f;

    private void Start()
    {
        Transform model = transform.GetChild(0);
        baseHeight = model.position.y;
    }

    protected override void ApplyEffect(Vehicle vehicle)
    {
        FindObjectOfType<Level>().AddBonus(value);
        base.ApplyEffect(vehicle);
    }

    private void Update()
    {
        Transform model = transform.GetChild(0);

        transform.Rotate(Vector3.up, spinSpeed * 360.0f * Time.deltaTime);
        floatS += floatSpeed * Time.deltaTime;
        model.position = new Vector3(transform.position.x, baseHeight + Mathf.SmoothStep(0.0f, 1.0f, Mathf.PingPong(floatS, 1.0f)), transform.position.z);
    }
}
