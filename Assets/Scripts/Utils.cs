using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static System.Random rnd = new System.Random();

    public static Bounds PointsToBounds(Vector3 point1, Vector3 point2)
    {
        Bounds result = new Bounds(point1, Vector3.zero);
        result.Encapsulate(point2);
        return result;
    }

    // Finds instance of component in instances list closest to position.
    public static T FindClosest<T>(IEnumerable<T> instances, Vector3 position) where T : MonoBehaviour
    {
        T closest = null;
        float closestDistance = float.PositiveInfinity;
        foreach (var instance in instances)
        {
            float distance = Vector3.Distance(instance.transform.position, position);
            if (distance < closestDistance)
            {
                closest = instance;
                closestDistance = distance;
            }
        }
        return closest;
    }

    // https://en.wikibooks.org/wiki/Linear_Algebra/Orthogonal_Projection_Onto_a_Line
    public static Vector3 ProjectPointOnLine(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {
        Vector3 v = point - linePoint1;
        Vector3 s = linePoint2 - linePoint1;
        return linePoint1 + Vector3.Dot(v, s) / Vector3.Dot(s, s) * s;
    }

    public static Vector3 DivideVectorsFields(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector3 MultiplyVectorsFields(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

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

    public static T RandomChoice<T>(T[] array)
    {
        if (array.Length == 0) return default(T);
        else return array[(uint)Random.Range(0, array.Length)];
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
}
