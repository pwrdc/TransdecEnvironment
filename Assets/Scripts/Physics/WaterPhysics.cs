using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    public enum BuoyancyForceMode
    {
        FullySimulated,
        FluctuationsOnly,
        Disabled
    }
    [HelpBox("This component will control rigidbody's properties like \n"
            +"drag, mass and gravity and add additional forces.\n"
            +"Body volume is calculated from the collider bounds and multiplied by Bounds Fill Level.", 
            HelpBoxMessageType.Info)]
    public BuoyancyForceMode buoyancyForceMode;
    bool FullySimulated => buoyancyForceMode == BuoyancyForceMode.FullySimulated;
    bool FluctuationsOnly => buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly;

    new Collider collider;
    public Bounds bounds=>collider!=null ? collider.bounds : new Bounds();
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
        collider = GetComponentInChildren<Collider>();
        body = GetComponent<Rigidbody>();
        body.drag = 1f;
        body.angularDrag = 1f;
        SetGravity();
    }

    float VolumeInWater(Vector3 point)
    {
        Vector3 lower = point + Vector3.up * bounds.extents.y;
        Vector3 upper = point - Vector3.up * bounds.extents.y;
        return 1f-Mathf.InverseLerp(lower.y, upper.y, Environment.Environment.Instance.GetWaterY());
    }

    float CalculateBuoyancyForce(Vector3 position, float volumePart)
    {
        if (!IsUnderWater(position))
        return 0f;
        float force;
        switch (buoyancyForceMode)
        {
            case BuoyancyForceMode.FullySimulated:
                force = BuoyancyForce.Instance.GetForce(position, volumePart);
                break;
            case BuoyancyForceMode.FluctuationsOnly:
                force = BuoyancyForce.Instance.GetFluctuations(position) * fluctuationsMagnitude;
                break;
            case BuoyancyForceMode.Disabled:
                throw new UnreachableCodeException();
            default:
                throw new InvalidEnumValueException(buoyancyForceMode);
        }
        return force * VolumeInWater(position);
    }

    void AddBuoyancyForce(Vector3 position, float volumePart)
    {
        if (IsUnderWater(position))// this additional check ensures that we won't add zero force
        {
            body.AddForceAtPosition(Vector3.up * CalculateBuoyancyForce(position, volumePart), position);
        }
    }

    IEnumerable<Vector3> ForcePositions => new Vector3[]
    {
        Vector3.zero,
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, -1),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, -1)
    }.Select(offset=>Utils.MultiplyVectorsFields(offset, bounds.extents) + bounds.center);
    float VolumePerOffset=> Volume / 9;

    void UpdateBuoyancyForces()
    {
        foreach(var offset in ForcePositions)
        {
            AddBuoyancyForce(offset, VolumePerOffset);
        }
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

        if (Environment.WaterCurrent.Instance.enabledInAcademy && waterCurrentEnabled && IsUnderWater(transform.position))
            AddCurrent ();
    }

    private void OnDrawGizmosSelected()
    {
        if (collider == null)
        {
            collider = GetComponentInChildren<Collider>();
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        foreach (var position in ForcePositions)
        {
            Gizmos.DrawRay(position, Vector3.up * CalculateBuoyancyForce(position, VolumePerOffset));
        }
    }
}
