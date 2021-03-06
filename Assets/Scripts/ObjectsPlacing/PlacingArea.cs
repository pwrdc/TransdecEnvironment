﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the area where placeables can be placed.
/// </summary>
public abstract class PlacingArea : MonoBehaviour
{
    public Vector3 CalculateBoundsSize(Placeable placeable)
    {
        Vector3 bounds = transform.lossyScale/2 - new Vector3(placeable.RadiusInDirection(transform.right), 0, placeable.RadiusInDirection(transform.forward));
        if (placeable.shape == Placeable.Shape.Sphere)
        {
            bounds.y -= placeable.RadiusInDirection(transform.up);
        }
        return bounds;
    }

    /// <summary>
    /// placeas placeable inside of the placing area
    /// will affect its transform rotation and transform position
    /// </summary>
    public abstract void Place(Placeable placeable);
    // returns true if placeable is inside of the placing area in its current state
    public abstract bool Contains(Placeable placeable);

    /// <summary>
    /// this method is invoked in placeable's OnDrawGizmosSelected method
    /// to show where the center of the placeable can be placed, 
    /// so that it is contained within the placeable area
    /// </summary>
    public abstract void DrawBoundsForPlaceable(Placeable placeable);
}
