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
    public OccupiedSpace occupiedSpace;
    public bool randomY;
    public float radius;
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
        public bool X;
        public bool Y;
        public bool Z;
        public Limit xLimit;
        public Limit yLimit;
        public Limit zLimit;
    }
    public RandomRotation randomRotation;
    public bool obscuresView;
    [System.Serializable]
    public class IntRange
    {
        public int min;
        public int max;
    }
    public IntRange count;

    public static bool Overlaps(Placeable a, Placeable b)
    {
        Vector3 aPosition = a.transform.position;
        Vector3 bPosition = b.transform.position;
        if (a.occupiedSpace==OccupiedSpace.InfiniteCyllinder || b.occupiedSpace == OccupiedSpace.InfiniteCyllinder)
        {
            // if either of them doesn't have randomY calculate distance between them on plane
            aPosition.y = bPosition.y = 0;
        }
        float radiuses = a.radius + b.radius;
        // avoid square rooting
        return Vector3.SqrMagnitude(aPosition-aPosition) >= radiuses*radiuses;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (occupiedSpace==OccupiedSpace.Sphere)
        {
            Gizmos.DrawWireSphere(transform.position, radius);
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
