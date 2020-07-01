using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Used to check if target is visible to the camera.
/// Should be added to the robot object.
/// When selected will display gizmos showing how the visibility is checked.
/// </summary>
[RequireComponent(typeof(Placeable))]
public class VisibilityChecker : MonoBehaviour
{
    Placeable placeable;
    public Placer placer;
    public Placeable target;

    void Start()
    {
        placeable = GetComponent<Placeable>();
    }

    public bool CheckPoint(Placeable placeable, Vector3 point)
    {
        Vector3 observerPosition = transform.position;

        Vector3 placeablePosition = placeable.transform.position;

        // create the smallest cuboid containing observer and target and check if placeable lies within it
        Bounds bounds = Utils.PointsToBounds(observerPosition, point);
        if (!bounds.Contains(placeable))
            return false;

        // check if distance from placeable to the line is greater than its radius in the direction of the line
        Vector3 projection = Utils.ProjectPointOnLine(observerPosition, point, placeablePosition);
        Vector3 direction = projection - placeablePosition;
        float radius = placeable.RadiusInDirection(direction);
        return (placeablePosition - projection).sqrMagnitude < radius * radius;
    }

    Vector3[] CheckedPoints(Placeable target)
    {
        return new Vector3[]
        {
            target.position+target.offset,
            TargetSide(target.transform.right),
            TargetSide(-target.transform.right),
            TargetSide(target.transform.up),
            TargetSide(-target.transform.up),
            TargetSide(target.transform.forward),
            TargetSide(-target.transform.forward)
        };
    }

    Vector3 TargetSide(Vector3 direction)
    {
        return target.position + target.offset + (direction * target.RadiusInDirection(direction));
    }

    bool ObscuresViewPreconditions(Placeable placeable, Placeable target)
    {
        return placeable != this.placeable && placeable != target && placeable.canObscureView;
    }

    // checks if placeable obscures view at the target
    // will also assign this.target to target for gizmos drawing
    public bool ObscuresView(Placeable placeable, Placeable target)
    {
        this.target = target;
        if (!ObscuresViewPreconditions(placeable, target))
            return false;
        Vector3[] checkedPoints = CheckedPoints(target);
        foreach (Vector3 point in checkedPoints)
        {
            if (CheckPoint(placeable, point))
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (placer == null || target==null) return;

        Vector3[] checkedPoints = CheckedPoints(target);
        foreach (Vector3 point in checkedPoints)
        {
            bool valid = placer.placed.TrueForAll((Placeable placeable) => !( ObscuresViewPreconditions(placeable, target) && CheckPoint(placeable, point)) );
            Gizmos.color = valid ? Color.yellow : Color.red;
            Gizmos.DrawLine(transform.position, point);
        }

        // draw target Gizmos too to save time switching between objects
        target.OnDrawGizmosSelected();
    }
}
