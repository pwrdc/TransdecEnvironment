using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public enum OccupiedSpace
    {
        InfiniteCyllinder,
        Sphere
    }
    public OccupiedSpace occupiedSpace=OccupiedSpace.Sphere;
    public enum Height
    {
        UnderSurface,
        InTheMiddle,
        OnBottom
    }
    public Height height;
    public float radius=1;
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
        float radiuses = a.radius + b.radius;
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
        bool rotateHorizontally = height != Height.OnBottom && occupiedSpace != OccupiedSpace.InfiniteCyllinder;
        if (rotateHorizontally && randomRotation.x) rotation.x = RotateAroundAxis(randomRotation.xLimit);
        if (randomRotation.y)                       rotation.y = RotateAroundAxis(randomRotation.yLimit);
        if (rotateHorizontally && randomRotation.z) rotation.z = RotateAroundAxis(randomRotation.zLimit);
    }

    // draws the area of Placeable and placing area bounds for placing this placeable
    // yellow means correct placement and red means conflict
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (placer != null)
        {
            placer.placingArea.DrawBoundsGizmo(this);
            if (!placer.Allowed(this))
            {
                Gizmos.color = Color.red;
            }
        }
        if (occupiedSpace==OccupiedSpace.Sphere)
        {
            Gizmos.DrawWireSphere(transform.position+offset, radius);
        } else
        {
            Matrix4x4 saved = Gizmos.matrix;
            Gizmos.matrix *= Matrix4x4.Translate(transform.position+offset);
            Gizmos.matrix *= Matrix4x4.Scale(new Vector3(radius*2, 0, radius * 2));
            Gizmos.DrawWireMesh(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cylinder));
            Gizmos.matrix = saved;
        }
    }
}
