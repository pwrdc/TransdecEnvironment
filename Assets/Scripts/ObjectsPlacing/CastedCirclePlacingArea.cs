﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a shape created by connecting two surfaces:
/// one is a horizontal circle scaled according to transform scale
/// and the other one is that circle casted down on all colliders belonging to layerMask.
/// In addition all placeables that are placed on the bottom 
/// are rotated to be parallel to the lower surface.
/// </summary>
public class CastedCirclePlacingArea : PlacingArea
{
    public LayerMask layerMask;

    struct BottomPoint
    {
        public float height;
        public Vector3 normal;
    }

    /// <summary>
    /// Casts a ray from upper surface at coordinates x and z
    /// and returns height at which the placeable can be placed 
    /// and the normal of the surface at this point
    /// </summary>
    BottomPoint Bottom(float x, float z, float placeableHeight)
    {
        Vector3 position = new Vector3(x, transform.position.y - placeableHeight, z);
        RaycastHit hit;
        Ray ray = new Ray(position, -transform.up);
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask.value))
            return new BottomPoint { height = hit.point.y + placeableHeight, normal=hit.normal};
        else
            // return a point right below the surface where the ray started
            return new BottomPoint { height = position.y, normal = Vector3.up };
    }

    public override void Place(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        float placeableHeight = placeable.RadiusInDirection(transform.up);

        Vector3 position = Random.insideUnitSphere;
        position.y = -placeableHeight;
        position.x *= bounds.x;
        position.z *= bounds.z;
        position += transform.position;

        if (placeable.verticalPlacement != Placeable.VerticalPlacement.UnderSurface)
        {
            BottomPoint bottom = Bottom(position.x, position.z, placeableHeight);

            switch (placeable.verticalPlacement)
            {
                case Placeable.VerticalPlacement.InTheMiddle:
                    position.y = Random.Range(position.y, bottom.height);
                    break;
                case Placeable.VerticalPlacement.OnBottom:
                    position.y = bottom.height;
                    // rotate the placeable to make it parallel to the ground in this point
                    placeable.transform.rotation =Quaternion.LookRotation(bottom.normal) * Quaternion.Euler(90, 0, 0) * placeable.transform.rotation;
                    break;
            }
        }
        placeable.transform.position=position- placeable.offset;
    }

    public override bool Contains(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        Vector3 position = placeable.transform.position-placeable.offset;
        float placeableHeight = placeable.RadiusInDirection(transform.up);
        if (position.y>transform.position.y+bounds.y || position.y <Bottom(position.x, position.z, placeableHeight).height)
        {
            return false;
        }
        Vector3 relativePosition = placeable.transform.position-transform.position;
        Vector3 normalizedPosition = Utils.DivideVectorsFields(relativePosition, bounds);
        normalizedPosition.y = 0;
        return normalizedPosition.sqrMagnitude<1;
    }

    void DrawCircle(float scaleX, float scaleZ)
    {
        Matrix4x4 saved = Gizmos.matrix;
        Gizmos.matrix *= Matrix4x4.Translate(transform.position);
        Gizmos.matrix *= Matrix4x4.Scale(new Vector3(scaleX, 0, scaleZ));
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = saved;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawCircle(transform.lossyScale.x, transform.lossyScale.z);
    }

    public override void DrawBoundsForPlaceable(Placeable placeable)
    {
        Gizmos.color = Contains(placeable) ? Color.yellow : Color.red;
        Vector3 radius = new Vector3(placeable.RadiusInDirection(transform.right), 0, placeable.RadiusInDirection(transform.forward));
        DrawCircle(transform.lossyScale.x-radius.x, transform.lossyScale.z-radius.z);
    }
}
