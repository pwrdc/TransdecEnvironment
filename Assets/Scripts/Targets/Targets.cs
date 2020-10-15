using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

[RequireComponent(typeof(ScenesGenerator))]
public class Targets : MonoBehaviour
{
    static Targets instance;
    public static Targets Instance=> Singleton.GetInstance(ref instance, instance => instance.Initialize());

    public GameObject[] targetsFolders;
    // used to create a dropdown in the inspector
    DropdownList<int> TargetFoldersNames()
    {
        DropdownList<int> result = new DropdownList<int>();
        if (targetsFolders != null)
        {
            for (int i = 0; i < targetsFolders.Length; i++)
            {
                result.Add(targetsFolders[i].name, i);
            }
        }
        return result;
    }
    [Dropdown("TargetFoldersNames")]
    public int selectedFolder;
    
    public bool useSettings = true;
    
    [ResetParameter] bool collectData = false;
    [ResetParameter] int focusedObject = 0;
    [ResetParameter("Positive")] bool positiveExamples = false;
    List<Target> targets=new List<Target>();

    void Awake()
    {
        Singleton.Initialize(this, ref instance);
        Initialize();
    }

    void Initialize()
    {
        ResetParameterAttribute.InitializeAll(this);
        if (Settings.Used)
            selectedFolder = Settings.TargetsFolderIndex;
        SetFoldersActivation();

        Transform folder = targetsFolders[selectedFolder].transform;
        GetComponent<ScenesGenerator>().targetsFolder = folder;
        ListTargets(folder);
    }

    void SetFoldersActivation()
    {
        for (int i = 0; i < targetsFolders.Length; i++)
        {
            targetsFolders[i].SetActive(i == selectedFolder);
        }
    }

    public class NoTargetsException : System.Exception {
        public NoTargetsException()
            : base("The selected targets folder needs to contain at least on target component instance.") { }
    }

    void ListTargets(Transform folder)
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

    public static List<Target> FindTargetsByName(string name)
    {
        List<Target> result = new List<Target>();
        foreach (var target in Instance.targets)
        {
            if (target.name == name)
                result.Add(target);
        }
        return result;
    }

    Target GetFocused()
    {
        if (collectData && !positiveExamples)
            return null;
        if (focusedObject >= 0 && focusedObject < Instance.targets.Count)
            return Instance.targets[focusedObject];
        else
            return null;
    }

    /// <summary>
    /// Returns currently focused target or null.
    /// </summary>
    public static Target Focused => Instance.GetFocused();

    public static int Count=> Instance.targets.Count;
}
