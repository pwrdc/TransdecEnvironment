using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool Contains(this Bounds bounds, Placeable placeable)
    {
        Vector3 position = placeable.position;
        return
            position.x + placeable.RadiusInDirection(-Vector3.left) >= bounds.min.x
         && position.y + placeable.RadiusInDirection(-Vector3.up) >= bounds.min.y
         && position.z + placeable.RadiusInDirection(-Vector3.forward) >= bounds.min.z
         && position.x + placeable.RadiusInDirection(-Vector3.left) <= bounds.max.x
         && position.y + placeable.RadiusInDirection(-Vector3.up) <= bounds.max.y
         && position.z + placeable.RadiusInDirection(-Vector3.forward) <= bounds.max.z;
    }

    public static Vector3 WithY(this Vector3 original, float y)
    {
        return new Vector3(original.x, y, original.z);
    }

    public static Vector3 XZ(this Vector3 original)
    {
        return new Vector3(original.x, 0, original.z);
    }

    public static Vector3 Select(this Vector3 original, System.Func<float, float> selector)
    {
        return new Vector3(selector(original.x), selector(original.y), selector(original.z));
    }

    public static float[] ToArray(this Vector3 vector)
    {
        return new float[] {
            vector.x,
            vector.y,
            vector.z
        };
    }

    public static int ToInt(this bool value) => value ? 1 : 0;
}
