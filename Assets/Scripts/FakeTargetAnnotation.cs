using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTargetAnnotation : MonoBehaviour
{
    [SerializeField]
    private GameObject target = null;
    [SerializeField]
    private float margin = 10.0f;
    [SerializeField]
    private Texture2D background;
    [SerializeField]
    private bool drawBox = false;
    [SerializeField]
    private Environment.WaterOpacity waterOpacity = null;
    private float probability = 0;
    [SerializeField]
    private int fps = 1;
    private bool isAnnotated = false;

    private Camera cam = null;

    private float[] boxCoord = new float[4];
    private GUIStyle style = null;
    [SerializeField]
    private GameObject pointsObj = null;
    private Vector3[] pts = new Vector3[8];

    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(UpdateProbability());
    }

    public void OnGUI()
    {
        if (isAnnotated)
            DrawOnCamera();
    }

    private IEnumerator UpdateProbability()
    {
        while (true)
        {
            probability = CalculateProbability();

            if (Utils.GetRandom(0.0f, 1.0f) < probability && probability > 0.7f)
                isAnnotated = true;
            else
                isAnnotated = false;

            yield return new WaitForSeconds(1 / fps);
        }
    }

    private float CalculateProbability()
    {
        List<GameObject> ptsObject = new List<GameObject>();
        Utils.GetObjectsInFolder(pointsObj, out ptsObject);
        Vector3[] points = new Vector3[8];
        List<Vector3> points2 = new List<Vector3>();
        for (int i = 0; i < ptsObject.Count; i++)
        {
            points[i] = new Vector3(ptsObject[i].transform.position.x, ptsObject[i].transform.position.y, ptsObject[i].transform.position.z);
        }
        Bounds bounds = Utils.GetComplexBounds(target);
        points2.Add(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        points2.Add(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));

        for (int i = 0; i < pts.Length; i++)
        {
            pts[i] = cam.WorldToViewportPoint(points[i]);
        }

        float result = 0;
        float maxPoints = (1 + 4) * 8;
        for (int i = 0; i < pts.Length; i++)
        {
            if (IsPointInCameraViewport(pts[i]))
            {
                if (IsPointVisible(points[i], Utils.GetDistance(cam.transform.position, pts[i])))
                {
                    float score = GetScoreForPoint(pts[i]);
                    if (score > 0)
                    {
                        result += 1;
                        result += (score * 4);
                    }
                }
            }
        }
        float probability = result / maxPoints;
        return probability;
    }

    private float GetScoreForPoint(Vector3 point)
    {
        float waterFog = waterOpacity.WaterFog;
        float maxDistance = 16.61f - 20.351f * waterFog;
        float minDistThatObjectIsVisible = 0.6f;
        float dist = point.z;
        float score = 1 - dist / maxDistance;
        if (score < 0)
            return -1;
        score += minDistThatObjectIsVisible;
        if (score > 1)
            score = 1;

        return score;
    }

    private bool IsPointVisible(Vector3 point, float distance)
    {
        int layerMask = ~(1 << 11);

        RaycastHit hit;

        Vector3 startPos = Camera.main.transform.position;
        if (Physics.Raycast(startPos, point - startPos, out hit, distance, layerMask))
        {
            return true;
        }
        return false;
    }

    private bool IsPointInCameraViewport(Vector3 pointInCameraView)
    {
        if (pointInCameraView.x < 0.93f && pointInCameraView.y < 0.93f && pointInCameraView.x > 0.03f && pointInCameraView.y > 0.03f)
            return true;
        return false;
    }

    void DrawOnCamera()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.box);
            style.normal.background = background;
        }
        Bounds bounds = Utils.GetComplexBounds(target);
        if (cam.WorldToScreenPoint(bounds.center).z < 0) return;
        pts[0] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[1] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        pts[2] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[3] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
        pts[4] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[5] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        pts[6] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[7] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
        for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;
        Vector3 min = pts[0];
        Vector3 max = pts[0];
        for (int i = 1; i < pts.Length; i++)
        {
            min = Vector3.Min(min, pts[i]);
            max = Vector3.Max(max, pts[i]);
        }
        boxCoord[0] = min.x;
        boxCoord[1] = min.y;
        boxCoord[2] = max.x;
        boxCoord[3] = max.y;

        if (drawBox)
        {
            Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            r.xMin -= margin;
            r.xMax += margin;
            r.yMin -= margin;
            r.yMax += margin;
            GUI.Box(r, "", style);
        }
    }
}
