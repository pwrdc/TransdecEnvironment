﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public enum OccupiedSpace
    {
        InfiniteCyllinder,
        Sphere
    }
    public OccupiedSpace occupiedSpace = OccupiedSpace.Sphere;
    public enum Height
    {
        UnderSurface,
        InTheMiddle,
        OnBottom
    }
    public Height height;
    public float radius = 1;
    public Vector3 scale = Vector3.one;
    public Vector3 offset;
    [System.Serializable]
    public class Limit
    {
        public bool enabled;
        public float min;
        public float max;
    }
    [System.Serializable]
    public class RandomRotation
    {
        public bool x;
        public bool y;
        public bool z;
        public Limit xLimit;
        public Limit yLimit;
        public Limit zLimit;
    }
    public RandomRotation randomRotation;
    public bool canObscureView;

    [HideInInspector]
    public Placer placer;

    [Tooltip("Used for debugging, there is a yellow line showing what is the allowed distance in this direction, and white one showing the direction itself.")]
    public Vector3 probingVector = Vector3.forward;

    Vector3 MultiplyVectorFields(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    Vector3 DivideVectorFields(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    // returns a vector from (0,0,0) to point on shape surface 
    // used for debugging RadiusInDirection
    // Sphere equation taken from: https://en.wikipedia.org/wiki/Sphere
    private Vector3 LeadingVector(Vector3 direction)
    {
        if (occupiedSpace == OccupiedSpace.InfiniteCyllinder)
        {
            direction.y = 0;
        }
        direction = Quaternion.Inverse(transform.rotation) * direction;
        direction = DivideVectorFields(direction, scale*radius);
        
        direction.Normalize();
        float alpha=Vector3.Angle(Vector3.forward, direction)*Mathf.Deg2Rad;
        Vector3 projectionOnXY = new Vector3(direction.x, direction.y, 0);
        float beta= Vector3.Angle(Vector3.right, projectionOnXY)*Mathf.Deg2Rad;

        Vector3 leadingVector = new Vector3();
        leadingVector.x = radius * scale.x * Mathf.Sin(alpha)*Mathf.Cos(beta);
        leadingVector.y = radius * scale.y * Mathf.Sin(alpha)*Mathf.Sin(beta);
        leadingVector.z = radius * scale.z * Mathf.Cos(alpha);
        return leadingVector;
    }

    public float RadiusInDirection(Vector3 direction)
    {
        return LeadingVector(direction).magnitude;
    }

    // checks if occupied spaces of two placeables overlap
    public static bool Overlaps(Placeable a, Placeable b)
    {
        Vector3 aPosition = a.transform.position;
        Vector3 bPosition = b.transform.position;
        if (a.occupiedSpace==OccupiedSpace.InfiniteCyllinder || b.occupiedSpace == OccupiedSpace.InfiniteCyllinder)
        {
            // if either of them is a infinite cylinder calculate distance between them on plane
            aPosition.y = bPosition.y = 0;
        }
        float radiuses = a.RadiusInDirection(aPosition-bPosition) + b.RadiusInDirection(aPosition - bPosition);
        // avoid square rooting
        return Vector3.SqrMagnitude(bPosition-aPosition) <= radiuses*radiuses;
    }

    // returns rotation around an axis taking the asisLimit into consideration if it is enabled
    private float RotateAroundAxis(Limit axisLimit)
    {
        if (randomRotation.xLimit.enabled)
            return Random.Range(axisLimit.min, axisLimit.max);
        else
            return Random.Range(0, 360);
    }

    // rotates placeable according to its randomRotation field
    public void RotateRandomly()
    {
        Vector3 rotation = new Vector3();
        // id doesn't make sense to rotate horizontally if some options are enabled
        bool rotateHorizontally = height != Height.OnBottom && height != Height.UnderSurface && occupiedSpace != OccupiedSpace.InfiniteCyllinder;
        if (rotateHorizontally && randomRotation.x) rotation.x = RotateAroundAxis(randomRotation.xLimit);
        if (randomRotation.y)                       rotation.y = RotateAroundAxis(randomRotation.yLimit);
        if (rotateHorizontally && randomRotation.z) rotation.z = RotateAroundAxis(randomRotation.zLimit);
    }

    // draws the area of Placeable and placing area bounds for placing this placeable
    // yellow means correct placement and red means conflict
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position + offset, probingVector);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + offset, transform.rotation*LeadingVector(probingVector));
        if (placer != null)
        {
            placer.placingArea.DrawBoundsGizmo(this);
            if (!placer.Allowed(this))
            {
                Gizmos.color = Color.red;
            }
        }
        Matrix4x4 saved = Gizmos.matrix;
        if (occupiedSpace==OccupiedSpace.Sphere)
        {
            Gizmos.matrix *= Matrix4x4.Translate(transform.position + offset);
            Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
            Gizmos.matrix *= Matrix4x4.Scale(scale);
            Gizmos.DrawWireSphere(Vector3.zero, radius);
        } else
        {
            Gizmos.matrix *= Matrix4x4.Translate(transform.position+offset);
            Gizmos.matrix *= Matrix4x4.Scale(new Vector3(radius*2 * scale.x, 0, radius * 2* scale.z));
            Gizmos.DrawWireMesh(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cylinder));
        }
        Gizmos.matrix = saved;
    }
}
