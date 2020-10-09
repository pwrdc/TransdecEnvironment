using UnityEngine;
using System.Collections;

public class WaterSurface : Singleton<WaterSurface>
{
    public static float Y => Instance == null ? Mathf.Infinity : Instance.transform.position.y;

    public static bool IsAbove(float y)
    {
        return y < Y;
    }
}
