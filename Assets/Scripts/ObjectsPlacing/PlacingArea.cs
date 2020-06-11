using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacingArea : MonoBehaviour
{
    public Vector3 CalculateBoundsSize(Placeable placeable)
    {
        Vector3 bounds = transform.lossyScale/2 - new Vector3(placeable.RadiusInDirection(transform.right), 0, placeable.RadiusInDirection(transform.forward));
        if (placeable.occupiedSpace == Placeable.OccupiedSpace.Sphere)
        {
            bounds.y -= placeable.RadiusInDirection(transform.up);
        }
        return bounds;
    }

    public abstract Vector3 RandomPosition(Placeable placeable);
    public abstract bool Contains(Placeable placeable);

    public abstract void DrawBoundsGizmo(Placeable placeable);
}
