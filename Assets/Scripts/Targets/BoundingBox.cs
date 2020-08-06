using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public Rect rect { get; private set; } = Rect.zero;

    void Update()
    {
        rect = CalculateRect();
    }

    Rect CalculateRect()
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
        // flip Y coordinates
        // for (int i = 0; i < points.Length; i++) points[i].y = 1 - points[i].y;

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
