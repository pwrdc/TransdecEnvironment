// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="Utils.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility functions
/// </summary>
public static class Utils
{
    /// <summary>
    /// The random
    /// </summary>
    private static System.Random rnd = new System.Random();

    /// <summary>
    /// Gets the random.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>System.Single.</returns>
    public static float GetRandom(float min, float max)
    {
        float randValue = (float)rnd.NextDouble();
        return (max - min) * randValue + min;
    }

    /// <summary>
    /// Gets the random.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>System.Int32.</returns>
    public static int GetRandom(int min, int max)
    {
        return rnd.Next(min, max);
    }

    /// <summary>
    /// Gets the complex bounds.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>Bounds.</returns>
    public static Bounds GetComplexBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds();
        bool firstRendFound = false;
        foreach (Renderer renderer in renderers)
        {
            if (firstRendFound)
                bounds.Encapsulate(renderer.bounds);
            else
            {
                bounds = renderer.bounds;
                firstRendFound = true;
            }
        }

        return bounds;
    }

    /// <summary>
    /// Gets the box coord.
    /// </summary>
    /// <param name="targetBounds">The target bounds.</param>
    /// <returns>Vector3[].</returns>
    public static Vector3[] GetBoxCoord(Bounds targetBounds)
    {
        Vector3[] boxCoord = new Vector3[2];
        Vector3[] pts = new Vector3[8];

        pts[0] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[1] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        pts[2] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[3] = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        pts[4] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[5] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);
        pts[6] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z);
        pts[7] = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z);

        Vector3 min = pts[0];
        Vector3 max = pts[0];

        for (int i = 1; i < pts.Length; i++)
        {
            min = Vector3.Min(min, pts[i]);
            max = Vector3.Max(max, pts[i]);
        }

        boxCoord[0] = new Vector3(min.x, min.y, min.z);
        boxCoord[1] = new Vector3(max.x, max.y, max.z);

        return boxCoord;
    }

    /// <summary>
    /// Changes the color of the object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="maxHueChange">The maximum hue change.</param>
    /// <param name="maxSaturationChange">The maximum saturation change.</param>
    /// <param name="maxValueChange">The maximum value change.</param>
    public static void ChangeObjectColor(GameObject obj, int maxHueChange, int maxSaturationChange, int maxValueChange)
    {
        MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mesh in meshRenderers)
        {
            Material[] materials = mesh.materials;
            foreach (Material material in materials)
            {
                Color color = material.color;
            }
        }
    }

    /// <summary>
    /// Calculates the equation of3 d line.
    /// </summary>
    /// <param name="pos1">The pos1.</param>
    /// <param name="pos2">The pos2.</param>
    /// <returns>Vector2[].</returns>
    public static Vector2[] calculateEquationOf3DLine(Vector3 pos1, Vector3 pos2)
    {
        Vector2 xyLine = calculateEquationOf2DLine(new Vector2(pos1.x, pos1.y), new Vector2(pos2.x, pos2.y));
        Vector2 xzLine = calculateEquationOf2DLine(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));

        return new Vector2[] { xyLine, xzLine };
    }

    /// <summary>
    /// Calculates the equation of2 d line.
    /// </summary>
    /// <param name="pos1">The pos1.</param>
    /// <param name="pos2">The pos2.</param>
    /// <returns>Vector2.</returns>
    public static Vector2 calculateEquationOf2DLine(Vector2 pos1, Vector2 pos2)
    {
        float a = (pos1.y - pos2.y) / (pos1.x - pos2.x);
        float b = pos1.y - pos1.x * a;
        return new Vector2(a, b);
    }

    /// <summary>
    /// Determines whether [is point in object] [the specified point].
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="lineEquationMin">The line equation minimum.</param>
    /// <param name="lineEquationMax">The line equation maximum.</param>
    /// <returns><c>true</c> if [is point in object] [the specified point]; otherwise, <c>false</c>.</returns>
    public static bool isPointInObject(Vector3 point, Vector2[] lineEquationMin, Vector2[] lineEquationMax)
    {
        if (lineEquationMin[0].x * point.x + lineEquationMin[0].y < point.y &&
            lineEquationMin[1].x * point.x + lineEquationMin[1].y < point.z &&
            lineEquationMax[0].x * point.x + lineEquationMax[0].y > point.y &&
            lineEquationMax[1].x * point.x + lineEquationMax[1].y > point.z)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Gets the objects and mesh in folder.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="otherObjs">The other objs.</param>
    /// <param name="otherObjsMesh">The other objs mesh.</param>
    public static void GetObjectsAndMeshInFolder(GameObject folder, out List<GameObject> otherObjs, out List<MeshRenderer[]> otherObjsMesh)
    {
        otherObjs = new List<GameObject>();
        otherObjsMesh = new List<MeshRenderer[]>();
        foreach (Transform child in folder.transform)
        {
            otherObjs.Add(child.gameObject);
            otherObjsMesh.Add(child.gameObject.GetComponentsInChildren<MeshRenderer>());
        }
    }

    /// <summary>
    /// Gets the objects in folder.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="otherObjs">The other objs.</param>
    public static void GetObjectsInFolder(GameObject folder, out List<GameObject> otherObjs)
    {
        otherObjs = new List<GameObject>();
        foreach (Transform child in folder.transform)
        {
            otherObjs.Add(child.gameObject);
        }
    }

    /// <summary>
    /// Clear environment mesh renderer for collecting data with enabled background image
    /// </summary>
    /// <param name="activate">Is transdec mesh activated</param>
    public static void ActivateEnvironmentMeshRenderer(bool activate)
    {
        GameObject transdec = GameObject.Find("Transdec");
        MeshRenderer[] meshRenderers = transdec.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = activate;
        }
    }

    /// <summary>
    /// Gets the distance.
    /// </summary>
    /// <param name="pos1">The pos1.</param>
    /// <param name="pos2">The pos2.</param>
    /// <returns>System.Single.</returns>
    public static float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y) + (pos1.y - pos2.y) * (pos1.y - pos2.y));
    }
}
