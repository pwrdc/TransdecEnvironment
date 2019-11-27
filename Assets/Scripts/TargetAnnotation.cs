// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
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


/// <summary>
/// Annotation of target
/// Draws and calculate box around target
/// </summary>
public class TargetAnnotation : MonoBehaviour
{
    /// <summary>
    /// The target
    /// </summary>
    private GameObject target;
    /// <summary>
    /// The margin
    /// </summary>
    [SerializeField]
    private float margin = 10.0f;
    /// <summary>
    /// The background
    /// </summary>
    [SerializeField]
    public Texture2D background;
    /// <summary>
    /// The draw box
    /// </summary>
    [SerializeField]
    private bool drawBox = false;
    /// <summary>
    /// The cam
    /// </summary>
    private Camera cam = null;

    /// <summary>
    /// The activate
    /// </summary>
    private bool activate = true;
    /// <summary>
    /// The activated mode
    /// </summary>
    private CameraType activatedMode;


    /// <summary>
    /// The box coord
    /// </summary>
    private float[] boxCoord = new float[4];
    /// <summary>
    /// The style
    /// </summary>
    private GUIStyle style = null;
    /// <summary>
    /// The PTS
    /// </summary>
    private Vector3[] pts = new Vector3[8];

    /// <summary>
    /// The number of image to display
    /// </summary>
    private int numOfImageToDisplay = 0;

    /// <summary>
    /// The file names
    /// </summary>
    private string[] fileNames;

    /// <summary>
    /// Draw box around target
    /// </summary>
    public void OnGUI()
    {
        if (activate)
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box);
                style.normal.background = background;
            }
            Bounds bounds = Utils.GetComplexBounds(target);
            // object is not visible
            if (cam.WorldToScreenPoint(bounds.center).z < 0) return;

            // 8 coordinates 
            //TODO: Change to Viewport, and adjust changes in pyTransdec
            pts[0] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[1] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[2] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[3] = cam.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[4] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[5] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            pts[6] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
            pts[7] = cam.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            // calculate coordinate for game view
            for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;
            
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

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake()
    {
        RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
        RobotAgent.Instance.OnDataUpdate += UpdateData;
        cam = RobotAgent.Instance.ActiveCamera;
    }

    /// <summary>
    /// Gets the bounding box.
    /// </summary>
    /// <returns>System.Single[].</returns>
    public float[] GetBoundingBox() { return boxCoord; }

    /// <summary>
    /// Updates the data.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void UpdateData(TargetSettings settings)
    {
        this.target = settings.targetAnnotation;
        this.drawBox = settings.drawBox;
    }

    /// <summary>
    /// Updates the data.
    /// </summary>
    public void UpdateData()
    {
        cam = RobotAgent.Instance.ActiveCamera;
    }
}
