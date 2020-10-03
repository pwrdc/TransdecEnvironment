using UnityEngine;
using System.Collections;

public class WaterSurface : MonoBehaviour
{
    static WaterSurface instance = null;
    public static WaterSurface Instance => Singleton.GetInstance(ref instance);

    public static float Y => Instance == null ? Mathf.Infinity : Instance.transform.position.y;

    public static bool IsAbove(float y)
    {
        return y < Y;
    }
}
