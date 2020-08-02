using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacingArea : PlacingArea
{

    public override void Place(Placeable placeable)
    {
        Vector3 bounds = CalculateBoundsSize(placeable);
        switch (placeable.horizontalPlacement)
        {
            case Placeable.HorizontalPlacement.Inside:
                placeable.transform.position = transform.position + new Vector3(Random.Range(-bounds.x, bounds.x), 0, Random.Range(-bounds.z, bounds.z));
                break;
            case Placeable.HorizontalPlacement.OnWall:
                PlaceOnWall(placeable, bounds);
                break;
            default:
                throw new InvalidEnumValueException(placeable.horizontalPlacement);
        }
        float y;
        switch (placeable.verticalPlacement)
        {
            case Placeable.VerticalPlacement.UnderSurface:
                y = transform.position.y + bounds.y;
                break;
            case Placeable.VerticalPlacement.InTheMiddle:
                y = transform.position.y + Random.Range(-bounds.y, bounds.y);
                break;
            case Placeable.VerticalPlacement.OnBottom:
                y = transform.position.y - bounds.y;
                break;
            default:
                throw new InvalidEnumValueException(placeable.horizontalPlacement);
        }
        placeable.transform.position=new Vector3(placeable.transform.position.x, y, placeable.transform.position.z);
        placeable.transform.position-=placeable.offset;
    }

    void PlaceOnWall(Placeable placeable, Vector3 bounds)
    {
        // placing on a side wall means that one coordinate is maximal 
        // and the other random between bounds
        int wall = Random.Range(0, 3);
        switch (wall)
        {
            case 0:
                placeable.position = transform.position + new Vector3(bounds.x, 0, Random.Range(-bounds.z, bounds.z));
                placeable.transform.rotation = Quaternion.LookRotation(transform.right, transform.up);
                break;
            case 1:
                placeable.position = transform.position + new Vector3(-bounds.x, 0, Random.Range(-bounds.z, bounds.z));
                placeable.transform.rotation = Quaternion.LookRotation(-transform.right, transform.up);
                break;
            case 2:
                placeable.position = transform.position + new Vector3(Random.Range(-bounds.x, bounds.x), 0, bounds.z);
                placeable.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
                break;
            case 3:
                placeable.position = transform.position + new Vector3(Random.Range(-bounds.x, bounds.x), 0, -bounds.z);
                placeable.transform.rotation = Quaternion.LookRotation(-transform.forward, transform.up);
                break;
            default:
                throw new UnreachableCodeException();
        }
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

    public override void DrawBoundsForPlaceable(Placeable placeable)
    {
        Gizmos.color = Contains(placeable) ? Color.yellow : Color.red;
        Gizmos.DrawWireCube(transform.position, CalculateBoundsSize(placeable)*2);
    }
}
