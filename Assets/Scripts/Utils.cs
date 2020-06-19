using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static System.Random rnd = new System.Random();

    // calls action until it returns true or it is called tries times
    // returns true if action succeded
    public static bool Try(int tries, System.Func<bool> action)
    {
        while (tries > 0)
        {
            if (action()) return true;
            else tries--;
        }
        return false;
    }

    public static float GetRandom(float min, float max)
    {
        float randValue = (float)rnd.NextDouble();
        return (max - min) * randValue + min;
    }

    public static int GetRandom(int min, int max)
    {
        return rnd.Next(min, max);
    }

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

    public static Vector2[] calculateEquationOf3DLine(Vector3 pos1, Vector3 pos2)
    {
        Vector2 xyLine = calculateEquationOf2DLine(new Vector2(pos1.x, pos1.y), new Vector2(pos2.x, pos2.y));
        Vector2 xzLine = calculateEquationOf2DLine(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));

        return new Vector2[] { xyLine, xzLine };
    }

    public static Vector2 calculateEquationOf2DLine(Vector2 pos1, Vector2 pos2)
    {
        float a = (pos1.y - pos2.y) / (pos1.x - pos2.x);
        float b = pos1.y - pos1.x * a;
        return new Vector2(a, b);
    }

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

    public static void GetObjectsInFolder(GameObject folder, out List<GameObject> otherObjs)
    {
        otherObjs = new List<GameObject>();
        foreach (Transform child in folder.transform)
        {
            otherObjs.Add(child.gameObject);
        }
    }

    public static void ActivateEnvironmentMeshRenderer(bool activate)
    {
        GameObject transdec = GameObject.FindWithTag("Environment");
        MeshRenderer[] meshRenderers = transdec.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = activate;
        }
    }

    public static float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y) + (pos1.y - pos2.y) * (pos1.y - pos2.y));
    }
}
