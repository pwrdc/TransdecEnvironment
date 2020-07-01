using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScenesGenerator))]
public class TargetsFolderSelector : MonoBehaviour
{
    ScenesGenerator scenesGenerator;
    public GameObject[] targetsFolders;
    public int selected;
    public bool useSettings = true;

    private void Awake()
    {
        if (InitializedSettings.IsMenu)
            selected = InitializedSettings.targetsFolderIndex;
        scenesGenerator = GetComponent<ScenesGenerator>();
        for(int i=0; i<targetsFolders.Length; i++)
        {
            targetsFolders[i].SetActive(i == selected);
        }
        Transform folder = targetsFolders[selected].transform;
        scenesGenerator.targetsFolder = folder;
        Target.ActivateFolder(folder);
    }
}
