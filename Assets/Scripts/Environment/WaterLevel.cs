using UnityEngine;

/// <summary>
/// Adding this component to a transform will set the global water level as its y coordinate.
/// If there is no instance of this component on the scene the water level is set to positive infinity.
/// </summary>
public class WaterLevel : Singleton<WaterLevel>
{
    public static float Y => Instance == null ? Mathf.Infinity : Instance.transform.position.y;

    public static bool IsAbove(float y)
    {
        return y < Y;
    }
}
