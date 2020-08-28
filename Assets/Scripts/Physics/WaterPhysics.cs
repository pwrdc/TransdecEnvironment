using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    public Bounds bounds;
    public float volume;
    Rigidbody body;

    void Reset()
    {
        bounds = Utils.GetComplexBounds(gameObject);
        volume = bounds.size.x * bounds.size.y * bounds.size.z;
    }
    
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.drag = 1f;
        body.angularDrag = 1f;
    }

    void AddBuoyancyForce(Vector3 offset, float volumePart)
    {
        Vector3 position = transform.TransformPoint(offset);
        if (Environment.Environment.Instance.isUnderwater(position.y))
        {
            body.AddForceAtPosition(Vector3.up * BuoyancyForce.Instance.GetForce(position, volumePart), position);
        }
    }

    void FixedUpdate()
    {
        // add forces to 9 points on the bounds casted onto local XZ plane

        float volumePart = volume / 9;

        AddBuoyancyForce(Vector3.zero, volumePart);

        AddBuoyancyForce(new Vector3(bounds.size.x, 0, bounds.size.z), volumePart);
        AddBuoyancyForce(new Vector3(-bounds.size.x, 0, bounds.size.z), volumePart);
        AddBuoyancyForce(new Vector3(bounds.size.x, 0, -bounds.size.z), volumePart);
        AddBuoyancyForce(new Vector3(-bounds.size.x, 0, -bounds.size.z), volumePart);
        
        AddBuoyancyForce(new Vector3(bounds.size.x, 0, 0), volumePart);
        AddBuoyancyForce(new Vector3(0, 0, bounds.size.z), volumePart);
        AddBuoyancyForce(new Vector3(-bounds.size.x, 0, 0), volumePart);
        AddBuoyancyForce(new Vector3(0, 0, -bounds.size.z), volumePart);

        if(WaterCurrent.Instance.isEnabled && Environment.Environment.Instance.isUnderwater(transform.position.y))
            body.AddForce(WaterCurrent.Instance.GetForce());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
