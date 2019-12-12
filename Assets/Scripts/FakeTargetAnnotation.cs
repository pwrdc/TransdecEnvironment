// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-26-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-28-2019
// ***********************************************************************
// <copyright file="FakeTargetAnnotation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates fake annotation
/// Now only for making videos for jury
/// </summary>
public class FakeTargetAnnotation : MonoBehaviour
{
    /// <summary>
    /// The target to annotate
    /// </summary>
    [SerializeField]
    private GameObject target = null;
    /// <summary>
    /// The margin of box around object
    /// </summary>
    [SerializeField]
    private float margin = 10.0f;
    /// <summary>
    /// Background texture of box
    /// </summary>
    [SerializeField]
    private Texture2D background;
    /// <summary>
    /// Is box show on GUI
    /// </summary>
    [SerializeField]
    private bool drawBox = false;
    /// <summary>
    /// The water opacity
    /// </summary>
    [SerializeField]
    private SceneEnvironment.WaterOpacity waterOpacity = null;
    /// <summary>
    /// Calculated probability of object to mark (fake ml model)
    /// </summary>
    private float probability = 0;
    /// <summary>
    /// The FPS
    /// </summary>
    [SerializeField]
    private int fps = 1;
    /// <summary>
    /// is object annotated, calculated via probability
    /// Normally it is done via ML Model
    /// </summary>
    private bool isAnnotated = false;

    /// <summary>
    /// The cam
    /// </summary>
    private Camera cam = null;

    /// <summary>
    /// The box coordinates
    /// </summary>
    private float[] boxCoord = new float[4];
    /// <summary>
    /// The style of box
    /// </summary>
    private GUIStyle style = null;
    /// <summary>
    /// The points object to look at
    /// </summary>
    [SerializeField]
    private GameObject pointsObj = null;
    /// <summary>
    /// temporary value of coordinates
    /// </summary>
    private Vector3[] pts = new Vector3[8];

    /// <summary>
    /// Initializes camera and probability.
    /// </summary>
    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(UpdateProbability());
    }

    /// <summary>
    /// Render when GUI is called
    /// </summary>
    public void OnGUI()
    {
        if(isAnnotated)
            DrawOnCamera();
    }

    /// <summary>
    /// Updates the probability.
    /// </summary>
    /// <returns>IEnumerator</returns>
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

    /// <summary>
    /// Calculate probability of image detection of object
    /// </summary>
    /// <returns>System.Single.</returns>
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
        //DrawOnCamera();
    }

    /// <summary>
    /// Gets the score for point.
    /// Calculated via distance from object and points seen
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>System.Single.</returns>
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

    /// <summary>
    /// Determines whether is point visible from camera using raycast
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="distance">The distance.</param>
    /// <returns><c>true</c> if is point visible from camera; otherwise, <c>false</c>.</returns>
    private bool IsPointVisible(Vector3 point, float distance)
    {
        int layerMask = ~(1 << 11);

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        Vector3 startPos = Camera.main.transform.position;
        if (Physics.Raycast(startPos, point - startPos, out hit, distance, layerMask))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether is point in camera viewport.
    /// </summary>
    /// <param name="pointInCameraView">The point in camera view.</param>
    /// <returns><c>true</c> if is point in camera viewport; otherwise, <c>false</c>.</returns>
    private bool IsPointInCameraViewport(Vector3 pointInCameraView)
    {
        if (pointInCameraView.x < 0.93f && pointInCameraView.y < 0.93f && pointInCameraView.x > 0.03f && pointInCameraView.y > 0.03f)
            return true;
        return false;
    }

    /// <summary>
    /// Draws box on camera.
    /// </summary>
    void DrawOnCamera()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.box);
            style.normal.background = background;
        }
        Bounds bounds = Utils.GetComplexBounds(target);
        // when object is not visible
        if (cam.WorldToScreenPoint(bounds.center).z < 0) return;
        // 8 coordinates 
        pts[0] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[1] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        pts[2] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[3] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
        pts[4] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[5] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
        pts[6] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
        pts[7] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
        // calculate coordinates in viewport
        for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;
        // max and min values
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
            // 'box'
            GUI.Box(r, "", style);
        }
    }
}
