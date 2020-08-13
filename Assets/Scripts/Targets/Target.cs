using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public CameraType cameraType;
    public GameObject annotation;
    [Header("This variable is set by Targets class")]
    public int index;

    private void Start()
    {
        if (annotation == null)
        {
            annotation = gameObject;
        }
    }
}
