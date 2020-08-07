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
    }
}
