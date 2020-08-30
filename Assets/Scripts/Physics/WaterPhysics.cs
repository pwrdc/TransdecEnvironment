using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    public enum BuoyancyForceMode
    {
        FullySimulated,
        FluctuationsOnly,
        Disabled
    }
    [HelpBox("This component will control rigidbody's properties like \ndrag, mass and gravity and add additional forces.", HelpBoxMessageType.Info)]
    public BuoyancyForceMode buoyancyForceMode;
    bool FullySimulated => buoyancyForceMode == BuoyancyForceMode.FullySimulated;
    bool FluctuationsOnly => buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly;

    public Bounds bounds;
    [ShowIf(EConditionOperator.Or, "FullySimulated", "deduceMassFromVolume")]
    [Range(0f, 1f), Tooltip("How much of the volume inside of the bounds is filled by the material.")]
    public float boundsFillLevel = 0.25f;
    [ShowIf("FluctuationsOnly")]
    public float fluctuationsMagnitude=1f;
    public bool waterCurrentEnabled = true;

    public bool deduceMassFromVolume=true;
    // Selected densities for different materials 
    // taken from https://en.wikipedia.org/wiki/Density#Densities
    public enum Density
    {
        Wood = 700,
        Plastic = 1175,
        Styrofoam = 75,
        Concrete = 2400,
        Aluminium = 2700,
        Iron = 7870,
        Air = 1,
        Water = 997,
        SetManually = 0
    }
    [ShowIf("deduceMassFromVolume")]
    public Density density=Density.Plastic;
    bool SetDensityManually => density == Density.SetManually;
    [ShowIf(EConditionOperator.And, "SetDensityManually", "deduceMassFromVolume")]
    public float densityManual = (float)Density.Plastic;

    [ShowNativeProperty]
    float Volume => bounds.size.x * bounds.size.y * bounds.size.z * boundsFillLevel;
    [ShowNativeProperty]
    float ActualDensity => SetDensityManually ? densityManual : (float)density;
    [ShowNativeProperty]
    float DeducedMass => Volume * ActualDensity;

    Rigidbody body;

    [Button("Recalculate Bounds")]
    void Reset()
    {
        bounds = Utils.GetComplexBounds(gameObject);
        bounds.center -= transform.position;
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
        float volumePart = Volume / 9;

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
        if (deduceMassFromVolume)
            body.mass = DeducedMass;

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
        Gizmos.DrawWireCube(transform.position+bounds.center, bounds.extents);
    }
}
