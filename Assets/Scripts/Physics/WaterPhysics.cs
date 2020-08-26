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
    }

    void Update()
    {
        body.AddForce(Vector3.up*BuoyancyForce.Instance.GetForce(transform.position, volume));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
