using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class WaterPhysics : MonoBehaviour
{
    #region inspector
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

    #endregion

    #region initialization

    Rigidbody body;

    void EnsureBounds()
    {
        if (bounds == null)
        {
            Collider collider = GetComponentInChildren<Collider>();
            bounds = collider != null ? collider.bounds : new Bounds();
            bounds.center -= transform.position;
        }
    }

    void Start()
    {
        EnsureBounds();
        body = GetComponent<Rigidbody>();
        body.drag = 1f;
        body.angularDrag = 1f;
        SetGravity();
    }

    bool IsUnderWater(Vector3 position) => Environment.Environment.Instance.IsUnderWater(position.y);

    void SetGravity()
    {
        if (buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly && IsUnderWater(transform.position))
            body.useGravity = false;
        else
            body.useGravity = true;
    }

    #endregion

    #region buoyancy

    struct VerticalBounds
    {
        public float lower;
        public float higher;
    }

    // here we calculate lowest and highest point for the central voxel 
    // because all voxels have the same size and same rotation 
    // we can just add these values to their centers to get their lowest and highest points 
    VerticalBounds GetVerticalBounds()
    {
        Vector3[] cubeCorners = new Vector3[]
        {
            new Vector3(1,   1,  1),
            new Vector3(-1,  1,  1),
            new Vector3(1,   1, -1),
            new Vector3(-1,  1, -1),
            new Vector3(1,  -1,  1),
            new Vector3(-1, -1,  1),
            new Vector3(1,  -1, -1),
            new Vector3(-1, -1, -1),
        };
        Vector3 cubeCornerToVoxelCorner(Vector3 corner)
        {
            // transform cube corner into bounds corner 
            corner = Utils.MultiplyVectorsFields(corner, bounds.extents);
            // there are three voxels in each direction, we use central one as a reference
            corner /= 3;
            // rotate the corner (this is the key step)
            corner = transform.rotation * corner;

            return corner;
        }
        IEnumerable<Vector3> centralVoxelCorners = cubeCorners.Select(cubeCornerToVoxelCorner);

        return new VerticalBounds
        {
            higher = centralVoxelCorners.Max(corner => corner.y),
            lower = centralVoxelCorners.Min(corner => corner.y)
        };
    }

    float VolumeInWater(Vector3 point, VerticalBounds verticalBounds)
    {
        Vector3 lower = point + Vector3.up * verticalBounds.lower;
        Vector3 upper = point - Vector3.up * verticalBounds.higher;
        return 1f-Mathf.InverseLerp(lower.y, upper.y, Environment.Environment.Instance.GetWaterY());
    }

    float CalculateBuoyancyForce(Vector3 position, float volumePart, VerticalBounds verticalBounds)
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
        return force * VolumeInWater(position, verticalBounds);
    }

    void AddBuoyancyForce(Vector3 position, float volumePart, VerticalBounds verticalBounds)
    {
        if (IsUnderWater(position))// this additional check ensures that we won't add zero force
        {
            body.AddForceAtPosition(Vector3.up * CalculateBuoyancyForce(position, volumePart, verticalBounds), position);
        }
    }

    // these points are analogous to rubic cube's parts' centers
    IEnumerable<Vector3> ForcePositions => new Vector3[]
    {
        new Vector3(0, -1, 0),
        new Vector3(1, -1, 1),
        new Vector3(-1, -1, 1),
        new Vector3(1, -1, -1),
        new Vector3(-1, -1, -1),
        new Vector3(1, -1, 0),
        new Vector3(0, -1, 1),
        new Vector3(-1, -1, 0),
        new Vector3(0, -1, -1),

        new Vector3(0, 0, 0),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, -1),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, -1),

        new Vector3(0, 1, 0),
        new Vector3(1, 1, 1),
        new Vector3(-1, 1, 1),
        new Vector3(1, 1, -1),
        new Vector3(-1, 1, -1),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 1),
        new Vector3(-1, 1, 0),
        new Vector3(0, 1, -1)
    }.Select(offset=>transform.TransformPoint(Utils.MultiplyVectorsFields(offset, bounds.extents*(2f/3f)) + bounds.center));
    float VolumePerOffset=> Volume / 27;

    void UpdateBuoyancyForces()
    {
        VerticalBounds verticalBounds = GetVerticalBounds();
        foreach(var offset in ForcePositions)
        {
            AddBuoyancyForce(offset, VolumePerOffset, verticalBounds);
        }
    }

    #endregion

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
        EnsureBounds();

        Gizmos.color = Color.cyan;
        
        // rotate and translate the matrix to draw the bounds
        Matrix4x4 saved=Gizmos.matrix;
        Gizmos.matrix *= Matrix4x4.Translate(transform.position + bounds.center);
        Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        // restore the matrix
        Gizmos.matrix = saved;

        if (buoyancyForceMode != BuoyancyForceMode.Disabled)
        {
            VerticalBounds verticalBounds = GetVerticalBounds();
            foreach (var position in ForcePositions)
            {
                // draw the buoyancy force
                Gizmos.DrawRay(position, Vector3.up * CalculateBuoyancyForce(position, VolumePerOffset, verticalBounds));
                // draw lower and upper bounds of the voxel
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(position + Vector3.up * verticalBounds.lower, position + Vector3.up * verticalBounds.higher);
                // restore the color
                Gizmos.color = Color.cyan;
            }
        }
    }
}
