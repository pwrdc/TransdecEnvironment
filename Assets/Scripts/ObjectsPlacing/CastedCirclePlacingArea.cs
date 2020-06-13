using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastedCirclePlacingArea : PlacingArea
{
    public LayerMask layerMask;

    float Bottom(float x, float z, float placeableHeight)
    {
        Vector3 position = new Vector3(x, transform.position.y - placeableHeight, z);
        RaycastHit hit;
        Ray ray = new Ray(position, -transform.up);
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask.value))
            return hit.point.y+placeableHeight;
        else
            // return a point right below the surface where the ray started
            return position.y;
    }

    public override Vector3 RandomPosition(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        float placeableHeight = placeable.RadiusInDirection(transform.up);

        Vector3 position = Random.insideUnitSphere;
        position.y = -placeableHeight;
        position.x *= bounds.x;
        position.z *= bounds.z;
        position += transform.position;

        if (placeable.verticalPlacement == Placeable.VerticalPlacement.UnderSurface)
            return position+placeable.offset;

        float bottom = Bottom(position.x, position.z, placeableHeight);
        
        switch (placeable.verticalPlacement)
        {
            case Placeable.VerticalPlacement.InTheMiddle:
                position.y = Random.Range(position.y, bottom);
                break;
            case Placeable.VerticalPlacement.OnBottom:
                position.y = bottom;
                break;
        }
        return position+ placeable.offset;
    }

    public override bool Contains(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        Vector3 position = placeable.transform.position-placeable.offset;
        float placeableHeight = placeable.RadiusInDirection(transform.up);
        if (position.y>transform.position.y+bounds.y || position.y <Bottom(position.x, position.z, placeableHeight))
        {
            return false;
        }
        Vector3 relativePosition = placeable.transform.position-transform.position;
        Vector3 normalizedPosition = Geometry.DivideVectorFields(relativePosition, bounds);
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

    public override void DrawBoundsGizmo(Placeable placeable)
    {
        Gizmos.color = Contains(placeable) ? Color.yellow : Color.red;
        Vector3 radius = new Vector3(placeable.RadiusInDirection(transform.right), 0, placeable.RadiusInDirection(transform.forward));
        DrawCircle(transform.lossyScale.x-radius.x, transform.lossyScale.z-radius.z);
    }
}
