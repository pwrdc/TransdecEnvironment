using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacingArea : MonoBehaviour
{
    public Vector3 CalculateBounds(Placeable placeable)
    {
        Vector3 bounds = transform.lossyScale - new Vector3(placeable.radius, 0, placeable.radius);
        if (placeable.occupiedSpace == Placeable.OccupiedSpace.Sphere)
        {
            bounds.y -= placeable.radius;
        }
        bounds /= 2;
        return bounds;
    }

    public abstract Vector3 RandomPosition(Placeable placeable);
}
