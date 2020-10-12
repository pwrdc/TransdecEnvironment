using UnityEngine;

/// <summary>
/// Class used to simplify turning classes inheriting directly from MonoBehaviour into Singletons.
/// For other classes use methods from static non-generic Singleton class.
/// </summary>
/// <example>
/// <code>
/// public class MySingletonClass : Singleton<MySingletonClass> { }
/// </code>
/// </example>
/// <typeparam name="T">Type of inheriting class.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance = default;
    public static T Instance => Singleton.GetInstance(ref instance);

    protected virtual void Start()
    {
        Singleton.Initialize((T)this, ref instance);
    }
}
