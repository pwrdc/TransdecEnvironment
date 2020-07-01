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

    private float[] boxCoord = new float[4];
    private GUIStyle style = null;
    private Vector3[] pts = new Vector3[8];

    public void OnGUI()
    {
        if (activate)
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box);
                style.normal.background = background;
            }
            Bounds bounds = Utils.GetComplexBounds(Target.Focused().gameObject);
            // object is not visible
            if (RobotAgent.Instance.ActiveCamera.WorldToScreenPoint(bounds.center).z < 0) return;

            // 8 coordinates 
            //TODO: Change to Viewport, and adjust changes in pyTransdec
            Camera cam=RobotAgent.Instance.ActiveCamera;
            pts[0] = cam.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[1] = cam.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[2] = cam.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[3] = cam.WorldToViewportPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[4] = cam.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[5] = cam.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[6] = cam.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[7] = cam.WorldToViewportPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            // calculate coordinate for game view
            for (int i = 0; i < pts.Length; i++) pts[i].y = 1 - pts[i].y;

            // calculate max and min values for drawing box
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
                Rect r = Rect.MinMaxRect(min.x * Screen.width, min.y * Screen.height, max.x * Screen.width, max.y * Screen.height);
                r.xMin -= margin;
                r.xMax += margin;
                r.yMin -= margin;
                r.yMax += margin;
                // 'box'
                GUI.Box(r, "", style);
            }
        }
    }

    public float[] GetBoundingBox() { return boxCoord; }
}
