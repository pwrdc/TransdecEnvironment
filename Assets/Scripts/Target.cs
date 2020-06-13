using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public CameraType cameraType;
    public GameObject annotation;

    private void Start()
    {
        if (annotation == null)
        {
            annotation = gameObject;
        }
        OnStart(this);
    }
    
    #region static variables and functions

    static bool initialized;
    static Transform container;
    static List<Target> targets;

    [ResetParameter] static bool collectData;
    [ResetParameter] static int focusedObject;
    [ResetParameter("Positive")] static bool positiveExamples;

    public static Target Focused()
    {
        if (!initialized) Initialize();
        if (collectData && !positiveExamples)
            return null;
        return targets[focusedObject];
    }

    public static Target AtIndex(int index)
    {
        if (!initialized) Initialize();
        return targets[index];
    }

    public static int Count()
    {
        if (!initialized) Initialize();
        return targets.Count;
    }

    static void Initialize()
    {
        Target target = FindObjectOfType<Target>();
        ResetParameterAttribute.InitializeAll(target);
        ListTargets(target.transform.parent);
    }

    static void OnStart(Target target)
    {
        if (!initialized)
        {
            ResetParameterAttribute.InitializeAll(target);
            ListTargets(target.transform.parent);
        }
        else
        {
            Debug.LogWarning("All targets should share the same parent for their ordering to work correctly.");
            targets.Add(target);
        }
    }

    static void ListTargets(Transform newContainer)
    {
        container = newContainer;
        targets = new List<Target>();

        // it is important to obtain targets in their order in the scene hierarchy
        // there is no such guarantee in FinObjectsOfType function
        for(int i=0; i<container.childCount; i++)
        {
            Transform child = container.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                Target target = child.GetComponent<Target>();
                if(target!=null)
                    targets.Add(target);
            }
        }
        initialized = true;
    }
    #endregion
}
