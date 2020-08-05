// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : piotrszleg
// Last Modified On : 09-03-2020
// ***********************************************************************
// <copyright file="TargetAnnotation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class TargetAnnotation : MonoBehaviour
{
    [SerializeField]
    private float margin = 10.0f;
    [SerializeField]
    public Texture2D background;
    [SerializeField]
    private bool drawBox = false;

    private bool activate = true;

    public float[] boundingBox { get; private set; } = new float[4];
    private GUIStyle style = null;

    public void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.box);
            style.normal.background = background;
        }
        Target focused = Targets.Focused;
        if (focused == null)
            return;
        Camera camera = RobotAgent.Instance.ActiveCamera;
        Bounds bounds = Utils.GetComplexBounds(focused.gameObject);
        // object is not visible
        if (camera.WorldToScreenPoint(bounds.center).z < 0)
            return;

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
        for (int i = 0; i < points.Length; i++) points[i].y = 1 - points[i].y;

        // calculate bounding box's min and max coordinates
        Vector3 min = points[0];
        Vector3 max = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        boundingBox[0] = min.x;
        boundingBox[1] = min.y;
        boundingBox[2] = max.x;
        boundingBox[3] = max.y;
        
        if (drawBox)
        {
            Rect r = Rect.MinMaxRect(min.x * Screen.width, min.y * Screen.height, max.x * Screen.width, max.y * Screen.height);
            r.xMin -= margin;
            r.xMax += margin;
            r.yMin -= margin;
            r.yMax += margin;
            // draw GUI box
            GUI.Box(r, "", style);
        }
    }
}
