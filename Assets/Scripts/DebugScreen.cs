﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{
    public GameObject canvas;
    public TargetAnnotation annotation;

    bool active;
    void Switch(bool newActive)
    {
        canvas.SetActive(newActive);
        annotation.enabled = newActive;
        active = newActive;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && Application.isEditor)
        {
            Switch(true);
            EditorApplication.isPaused = true;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Switch(!active);
        }
    }
}
