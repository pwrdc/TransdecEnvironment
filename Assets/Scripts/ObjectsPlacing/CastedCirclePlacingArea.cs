using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastedCirclePlacingArea : PlacingArea
{
    public override Vector3 RandomPosition(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);

        Vector3 position = Random.insideUnitSphere;
        position.y = 0;
        position.x *= bounds.x;
        position.y *= bounds.y;
        position += transform.position;

        float bottom = transform.position.y - bounds.y;
        RaycastHit hit;
        if (Physics.Raycast(position, -transform.up, out hit))
        {
            bottom = hit.point.y;
        }
        switch (placeable.verticalPlacement)
        {
            case Placeable.VerticalPlacement.UnderSurface:
                position.y = transform.position.y;
                break;
            case Placeable.VerticalPlacement.InTheMiddle:
                position.y = transform.position.y + Random.Range(0, hit.point.y);
                break;
            case Placeable.VerticalPlacement.OnBottom:
                position.y = hit.point.y;
                break;
        }
        position += placeable.offset;
        return position;
    }

    public override bool Contains(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        return Vector3.Distance(placeable.transform.position, transform.position) < bounds.magnitude;
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 saved = Gizmos.matrix;
        Gizmos.color = Color.yellow;
        Gizmos.matrix *= Matrix4x4.Translate(transform.position);
        Vector3 scale = transform.lossyScale;
        scale.y = 0;
        Gizmos.matrix *= Matrix4x4.Scale(scale);
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = saved;
    }

    public override void DrawBoundsGizmo(Placeable placeable)
    {

    }
}
