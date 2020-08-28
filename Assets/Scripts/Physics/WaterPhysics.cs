using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    [HelpBox("This component will modify some of rigidbody's properties.\nSee docstring for more information.", HelpBoxMessageType.Info)]
    public Bounds bounds;
    public float volume;
    Rigidbody body;

    public bool waterCurrentEnabled = true;
    public enum BuoyancyForceMode
    {
        FullySimulated,
        FluctuationsOnly,
        Disabled
    }
    public BuoyancyForceMode buoyancyForceMode;

    void Reset()
    {
        bounds = Utils.GetComplexBounds(gameObject);
        volume = bounds.size.x * bounds.size.y * bounds.size.z;
    }

    void SetGravity()
    {
        if (buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly)
            body.useGravity = false;
    }
    
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.drag = 1f;
        body.angularDrag = 1f;
        SetGravity();
    }

    void AddBuoyancyForce(Vector3 offset, float volumePart)
    {
        Vector3 position = transform.TransformPoint(offset);
        if (Environment.Environment.Instance.isUnderwater(position.y))
        {
            float force;
            switch (buoyancyForceMode) {
                case BuoyancyForceMode.FullySimulated:
                    force = BuoyancyForce.Instance.GetForce(position, volumePart);
                    break;
                case BuoyancyForceMode.FluctuationsOnly:
                    force = BuoyancyForce.Instance.GetForce(position, volumePart);
                    break;
                case BuoyancyForceMode.Disabled:
                    throw new UnreachableCodeException();
                default:
                    throw new InvalidEnumValueException(buoyancyForceMode);
            }
            body.AddForceAtPosition(Vector3.up * force, position);
        }
    }

    void UpdateBuoyancyForces()
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
    }

    void AddCurrent()
    {
        body.AddForce(WaterCurrent.Instance.GetForce());
    }

    void FixedUpdate()
    {
        SetGravity();

        if (buoyancyForceMode != BuoyancyForceMode.Disabled)
        {
            UpdateBuoyancyForces();
        }

        if (WaterCurrent.Instance.isEnabled && waterCurrentEnabled && Environment.Environment.Instance.isUnderwater(transform.position.y))
            AddCurrent ();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
