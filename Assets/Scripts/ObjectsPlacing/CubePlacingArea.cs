using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacingArea : PlacingArea
{
    public override Vector3 RandomPosition(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        Vector3 position = transform.position + new Vector3(Random.Range(-bounds.x, bounds.x), 0, Random.Range(-bounds.z, bounds.z));
        switch (placeable.verticalPlacement)
        {
            case Placeable.VerticalPlacement.UnderSurface:
                position.y = transform.position.y + bounds.y;
                break;
            case Placeable.VerticalPlacement.InTheMiddle:
                position.y = transform.position.y + Random.Range(-bounds.y, bounds.y);
                break;
            case Placeable.VerticalPlacement.OnBottom:
                position.y = transform.position.y - bounds.y;
                break;
        }
        position -= placeable.offset;
        return position;
    }

    public override bool Contains(Placeable placeable)
    {
        Vector3 boundsSize = CalculateBoundsSize(placeable);
        Bounds bounds = new Bounds(transform.position, boundsSize*2);
        return bounds.Contains(placeable.transform.position+placeable.offset);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }

    public override void DrawBoundsGizmo(Placeable placeable)
    {
        Gizmos.color = Contains(placeable) ? Color.yellow : Color.red;
        Gizmos.DrawWireCube(transform.position, CalculateBoundsSize(placeable)*2);
    }
}
