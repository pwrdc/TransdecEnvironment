using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocator : MonoBehaviour
{
    Transform target;
    public Rect ScreenRect { get; private set; } = Rect.zero;
    public Vector3 RelativePosition { get; private set; } = Vector3.zero;
    public float RelativeAngle { get; private set; }

    public void UpdateValues()
    {
        target = Targets.Focused?.transform;
        if (target != null)
        {
            ScreenRect = CalculateScreenRect();
            RelativePosition=CalculateRelativePosition();
            RelativePosition = CalculateRelativePosition();
            RelativeAngle=CalculateRelativeAngle();
        } else
        {
            ScreenRect = Rect.zero;
            RelativePosition = Vector3.zero;
            RelativeAngle = 0.0f;
        }
    }

    public Vector3 CalculateRelativePosition()
    {
        Transform target = Targets.Focused?.transform;
        if (target == null)
        {
            return Vector3.zero;
        }
        Vector3 distToCenter = target.InverseTransformPoint(target.position);
        Vector3 relativePos = target.InverseTransformPoint(transform.position) - distToCenter;
        relativePos.x = Mathf.Abs(relativePos.x);
        relativePos.y = Mathf.Abs(relativePos.y);
        relativePos.z = Mathf.Abs(relativePos.z);
        return relativePos;
    }

    public float CalculateRelativeAngle()
    {
        Transform target = Targets.Focused?.transform;
        if (target == null)
        {
            return 0f;
        }
        float relativeYaw = (Quaternion.Inverse(target.rotation) * transform.rotation).eulerAngles.y;
        relativeYaw = Mathf.Abs((relativeYaw + 180) % 360 - 180);
        return relativeYaw;
    }

    Rect CalculateScreenRect()
    {
        Target focused = Targets.Focused;
        if (focused == null)
            return Rect.zero;
        Camera camera = RobotAgent.Instance.ActiveCamera;
        Bounds bounds = Utils.GetComplexBounds(focused.gameObject);
        // object is not visible
        if (camera.WorldToScreenPoint(bounds.center).z < 0)
            return Rect.zero;

        Vector3[] points = new Vector3[8];
        points[0] = camera.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        points[1] = camera.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        points[2] = camera.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        points[3] = camera.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
        points[4] = camera.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        points[5] = camera.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        points[6] = camera.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        points[7] = camera.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));

        // calculate bounding box's min and max coordinates
        Vector3 min = points[0];
        Vector3 max = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
}
