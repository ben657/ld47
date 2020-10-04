using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    public UnityEvent OnPickedUp = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponentInParent<IPlayerController>();

        if (controller != null)
            ApplyEffect(controller.Vehicle);
    }

    protected virtual void ApplyEffect(Vehicle vehicle)
    {
        OnPickedUp.Invoke();
        Destroy(gameObject);
    }

    public float GetSize()
    {
        return GetComponent<SphereCollider>().radius * 2.0f;
    }
}
