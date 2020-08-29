using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    public enum BuoyancyForceMode
    {
        FullySimulated=0,
        FluctuationsOnly,
        Disabled
    }
    [HelpBox("This component will modify some of rigidbody's properties.\nSee docstring for more information.", HelpBoxMessageType.Info)]
    public BuoyancyForceMode buoyancyForceMode;
    bool FullySimulated => buoyancyForceMode == BuoyancyForceMode.FullySimulated;
    bool FluctuationsOnly => buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly;
    [ShowIf("FullySimulated")]
    public Bounds bounds;
    [ShowIf("FullySimulated")]
    public float volume;
    [ShowIf("FluctuationsOnly")]
    public float fluctuationsMagnitude=1f;
    Rigidbody body;

    public bool waterCurrentEnabled = true;

    void Reset()
    {
        bounds = Utils.GetComplexBounds(gameObject);
        volume = bounds.size.x * bounds.size.y * bounds.size.z;
    }

    bool IsUnderWater(Vector3 position) => Environment.Environment.Instance.IsUnderWater(position.y);

    void SetGravity()
    {
        if (buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly && IsUnderWater(transform.position))
            body.useGravity = false;
        else
            body.useGravity = true;
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
        if (IsUnderWater(position))
        {
            float force;
            switch (buoyancyForceMode) {
                case BuoyancyForceMode.FullySimulated:
                    force = BuoyancyForce.Instance.GetForce(position, volumePart);
                    break;
                case BuoyancyForceMode.FluctuationsOnly:
                    force = BuoyancyForce.Instance.GetFluctuations(position)*fluctuationsMagnitude;
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
        body.AddForce(Environment.WaterCurrent.Instance.GetForce());
    }

    void FixedUpdate()
    {
        SetGravity();

        if (buoyancyForceMode != BuoyancyForceMode.Disabled)
        {
            UpdateBuoyancyForces();
        }

        if (Environment.WaterCurrent.Instance.isEnabled && waterCurrentEnabled && IsUnderWater(transform.position))
            AddCurrent ();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
