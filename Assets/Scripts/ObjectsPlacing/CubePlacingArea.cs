using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacingArea : PlacingArea
{
    public override Vector3 RandomPosition(Placeable placeable)
    {
        Vector3 bounds = CalculateBounds(placeable);
        Vector3 position = transform.position + new Vector3(Random.Range(-bounds.x, bounds.x), 0, Random.Range(-bounds.z, bounds.z));
        switch (placeable.height)
        {
            case Placeable.Height.UnderSurface:
                position.y = transform.position.y + bounds.y;
                break;
            case Placeable.Height.InTheMiddle:
                position.y = transform.position.y + Random.Range(-bounds.y, bounds.y);
                break;
            case Placeable.Height.OnBottom:
                position.y = transform.position.y - bounds.y;
                break;
        }
        position += placeable.offset;
        return position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
