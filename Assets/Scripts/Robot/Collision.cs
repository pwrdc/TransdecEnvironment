using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Collision : MonoBehaviour
{
    List<Collider> colliders = new List<Collider>();
    [ReadOnly]
    public float closestDistance;
    Collider closestCollider;

    void OnTriggerEnter(Collider collider)
    {
        if (!colliders.Contains(collider))
        {
            colliders.Add(collider);
            float distance = Vector3.Distance(collider.transform.position, transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestCollider = collider;
            }
        }
        EventsLogger.Log("Collision with an object.");
    }

    void OnTriggerExit(Collider collider)
    {
        colliders.Remove(collider);
        if (closestCollider == collider)
        {
            FindClosestCollider();
        }
    }

    void FindClosestCollider()
    {
        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(collider.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }
    }
}
