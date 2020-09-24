using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

/// <summary>
/// Adds buoyancy force to a rigidbody and controls its gravity.
/// This class is meant to be inherited by WaterPhysics and can't be instantiated directly.
/// In this case inheritance is used not for code sharing but to increase the code readability. 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class BuoyancyPhysics : MonoBehaviour
{
    public enum BuoyancyForceMode
    {
        FullySimulated,
        // use this mode for bodies suspended in water (not moving up or down due to gravity and buoyancy being balanced)
        FluctuationsOnly,
        Disabled
    }
    [HelpBox("This component will control rigidbody's properties like \n"
            + "drag, mass and gravity and add additional forces.\n"
            + "Body volume is calculated from the collider bounds and multiplied by Bounds Fill Level.",
            HelpBoxMessageType.Info)]
    public BuoyancyForceMode buoyancyForceMode;
    bool FullySimulated => buoyancyForceMode == BuoyancyForceMode.FullySimulated;
    bool FluctuationsOnly => buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly;

    public Bounds bounds;
    [ShowIf(EConditionOperator.Or, "FullySimulated", "deduceMassFromVolume")]
    [Range(0f, 1f), Tooltip("How much of the volume inside of the bounds is filled by the material.")]
    public float boundsFillLevel = 0.25f;
    [ShowIf("FluctuationsOnly")]
    public float fluctuationsMagnitude = 1f;

    [Tooltip("The higher this value is the better the quality of the simulation will be.\n" +
             "9 is sensible default, 4 can be used for smaller objects.\n" +
             "For centrally symmetrical objects (like spheres) there is no need for more than 1.")]
    public int voxelsPerDimension = 3;

    [ShowNativeProperty]
    protected float Volume => bounds.size.x * bounds.size.y * bounds.size.z * boundsFillLevel;
    [ShowNativeProperty]
    protected int VoxelsCount => voxelsPerDimension * voxelsPerDimension * voxelsPerDimension;

    protected Rigidbody body;
    
    void Start()
    {
        EnsureBounds();
        body = GetComponent<Rigidbody>();
        body.drag = 1f;
        body.angularDrag = 1f;
        SetGravity();
    }

    void EnsureBounds()
    {
        if (bounds == null)
        {
            CalculateBounds();
        }
    }

    void CalculateBounds()
    {
        Collider collider = GetComponentInChildren<Collider>();
        bounds = collider != null ? collider.bounds : new Bounds();
        bounds.center -= transform.position;
        bounds.size = Utils.DivideVectorsFields(bounds.size, transform.lossyScale);
    }

    void Reset()
    {
        CalculateBounds();
    }

    void SetGravity()
    {
        if (buoyancyForceMode == BuoyancyForceMode.FluctuationsOnly && IsUnderWater(transform.position))
            body.useGravity = false;
        else
            body.useGravity = true;
    }

    // represents higher and lower bounds of a voxel in Y axis
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
            // there are voxelsPerDimension voxels in each direction, we use central one as a reference
            corner /= voxelsPerDimension;
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
        return 1f - Mathf.InverseLerp(lower.y, upper.y, Environment.Environment.Instance.GetWaterY());
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

    protected bool IsUnderWater(Vector3 position) => Environment.Environment.Instance.IsUnderWater(position.y);

    Vector3[] GetForcePositions()
    {
        // there is no caching because the positions depend on constantly changing rotation
        // iterating over them and transforing them would have the same cost as creating them from scratch
        Vector3[] result = new Vector3[VoxelsCount];
        int index = 0;
        IterateAxis(bounds.size.x, voxelsPerDimension, x =>
            IterateAxis(bounds.size.y, voxelsPerDimension, y =>
                IterateAxis(bounds.size.z, voxelsPerDimension, z =>
                    result[index++] = ProcessForcePoint(new Vector3(x, y, z)))));
        return result;
    }

    // for loop for iterating over force positions is very complicated
    // so here it is extracted into a separate function
    void IterateAxis(float size, int divider, System.Action<float> body)
    {
        float step = size / divider;
        float start = -size / 2 + step / 2;
        for (int i = 0; i < divider; i++)
        {
            body(start + i * step);
        }
    }

    Vector3 ProcessForcePoint(Vector3 offset)
    {
        return transform.TransformPoint(offset + bounds.center);
    }

    float VolumePerForcePosition => Volume / VoxelsCount;

    protected void UpdateBuoyancyForces()
    {
        VerticalBounds verticalBounds = GetVerticalBounds();
        foreach (var offset in GetForcePositions())
        {
            AddBuoyancyForce(offset, VolumePerForcePosition, verticalBounds);
        }
    }

    protected virtual void FixedUpdate()
    {
        SetGravity();

        if (buoyancyForceMode != BuoyancyForceMode.Disabled)
        {
            UpdateBuoyancyForces();
        }
    }

    private void OnDrawGizmosSelected()
    {
        EnsureBounds();

        Gizmos.color = Color.cyan;

        // rotate and translate the matrix to draw the bounds
        Matrix4x4 saved = Gizmos.matrix;
        Gizmos.matrix *= Matrix4x4.Translate(transform.position + bounds.center);
        Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
        Gizmos.matrix *= Matrix4x4.Scale(transform.lossyScale);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        // restore the matrix
        Gizmos.matrix = saved;

        if (buoyancyForceMode != BuoyancyForceMode.Disabled)
        {
            VerticalBounds verticalBounds = GetVerticalBounds();
            foreach (var position in GetForcePositions())
            {
                // draw the buoyancy force
                Gizmos.DrawRay(position, Vector3.up * CalculateBuoyancyForce(position, VolumePerForcePosition, verticalBounds));
                // draw lower and upper bounds of the voxel
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(position + Vector3.up * verticalBounds.lower, position + Vector3.up * verticalBounds.higher);
                // restore the color
                Gizmos.color = Color.cyan;
            }
        }
    }
}
