using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScenesGenerator))]
public class Targets : MonoBehaviour
{
    public GameObject[] targetsFolders;
    public int selected;
    public bool useSettings = true;
    
    // these are static to simplify the static methods
    [ResetParameter] static bool collectData = false;
    [ResetParameter] static int focusedObject = 0;
    [ResetParameter("Positive")] static bool positiveExamples = false;
    static List<Target> targets=new List<Target>();
    static Targets instance=null;

    private void Awake()
    {
        if (instance != null)
        {
            throw new MultipleInstancesException();
        }
        instance = this;

        ResetParameterAttribute.InitializeAll(this);
        if (Settings.Used)
            selected = Settings.TargetsFolderIndex;
        SetFoldersActivation();

        Transform folder = targetsFolders[selected].transform;
        GetComponent<ScenesGenerator>().targetsFolder = folder;
        ListTargets(folder);
    }

    public static List<Target> FindTargetsByName(string name)
    {
        List<Target> result=new List<Target>();
        foreach (var target in targets)
        {
            if (target.name == name)
                result.Add(target);
        }
        return result;
    }

    void SetFoldersActivation()
    {
        for (int i = 0; i < targetsFolders.Length; i++)
        {
            targetsFolders[i].SetActive(i == selected);
        }
    }

    public class NoTargetsException : System.Exception {
        public NoTargetsException()
            : base("The selected targets folder needs to contain at least on target component instance.") { }
    }

    static void ListTargets(Transform folder)
    {
        targets.Clear();
        // it is important to obtain targets in their order in the scene hierarchy
        // there is no such guarantee in FindObjectsOfType function
        for (int i = 0; i < folder.childCount; i++)
        {
            Transform child = folder.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                Target target = child.GetComponent<Target>();
                if (target != null)
                {
                    target.index = targets.Count;
                    targets.Add(target);
                }
            }
        }
        if (targets.Count == 0)
            throw new NoTargetsException();
    }

    public static Target Focused
    {
        get
        {
             if (collectData && !positiveExamples)
                 return null;
             if (focusedObject >= 0 && focusedObject < targets.Count)
                 return targets[focusedObject];
             else
                 return null;
        }
    }

    public static int Count=> targets.Count;
}
