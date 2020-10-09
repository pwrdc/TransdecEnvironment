using UnityEngine;

/// <summary>
/// This class provides static methods used to turn classes into singletons
/// with proper exceptions for all incorrect singleton usage cases.
/// </summary>
public static class Singleton
{
    public class NoSingletonInstanceException : System.Exception {
        public NoSingletonInstanceException(System.Type type)
            : base($"There is no instance of Singleton class {type}.") { }
    };
    public class MultipleSingletonInstancesException : System.Exception {
        public MultipleSingletonInstancesException(System.Type type)
            : base($"There are multiple instances of Singleton class {type}.") { }
    };

    /// <summary>
    /// Function that is meant to be used in Singleton's Instance property getter.
    /// </summary>
    /// <example>
    /// <code>
    /// static MySingletonClass instance;
    /// public static MySingletonClass Instance => Singleton.GetInstance(ref instance);
    /// // or with initializer:
    /// static MySingletonClass instance;
    /// public static MySingletonClass Instance => Singleton.GetInstance(ref instance, instance => instance.Initialize());
    /// </code>
    /// </example>
    /// <typeparam name="T">Singleton class.</typeparam>
    /// <param name="instanceField">Private static field of the singleton class where the instance will be cached.</param>
    /// <param name="initializer">Optional function that will be called on newly found instance before passing it to the property user.</param>
    /// <returns>Instance of the class.</returns>
    /// <remarks>
    /// Initializer argument will be called on the object only if it is found using FindObjectOfType in this function before its Singleton.Initialize call.
    /// It is used to transform the object to its usable state in case its instance is needed before its Start or Awake function is called.
    /// Call to Singleton.Initialize in initializer is allowed but uneccesary.
    /// </remarks>
    public static T GetInstance<T>(ref T instanceField, System.Action<T> initializer=null) where T : MonoBehaviour
    {
        if (instanceField != null)
        {
            return instanceField;
        }
        else
        {
            instanceField = Object.FindObjectOfType<T>();
            if (instanceField == null)
            {
                throw new NoSingletonInstanceException(typeof(T));
            }
            else
            {
                initializer?.Invoke(instanceField);
                return instanceField;
            }
        }
    }

    /// <summary>
    /// Function that is meant be called in Singleton's Start or Awake functions.
    /// </summary>
    /// <example>
    /// <code>
    /// void Awake()
    /// {
    ///    Singleton.Initialize(this, ref instance);
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">Singleton class.</typeparam>
    /// <param name="this">Instance of the class available as this in the caller function.</param>
    /// <param name="instanceField">Private static field of the singleton class where the instance will be cached.</param>
    /// <remarks>
    /// Singleton classes that don't call this function will work 
    /// but it makes finding the static instance faster 
    /// (field assignment instead of expensive FindObjectOfType call)
    /// and throws an error if there is more than one instance of singleton class on the scene.
    /// </remarks>
    public static void Initialize<T>(T @this, ref T instanceField) where T : MonoBehaviour
    {
        if (instanceField == null)
        {
            instanceField = @this;
        } else if(instanceField!=@this)
        {
            throw new MultipleSingletonInstancesException(typeof(T));
        }
    }

    public static void Deinitialize<T>(T @this, ref T instanceField) where T : MonoBehaviour
    {
        if (instanceField == @this)
        {
            instanceField = null;
        }
    }
}