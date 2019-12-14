// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-28-2019
// ***********************************************************************
// <copyright file="RobotAcademy.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Objects;

/// <summary>
/// Robot control
/// </summary>
public enum RobotControl
{
    /// <summary>
    /// Player script
    /// </summary>
    player,
    /// <summary>
    /// Python controller
    /// </summary>
    python,
    /// <summary>
    /// Pad controller
    /// </summary>
    pad,
    /// <summary>
    /// Python without image
    /// </summary>
    pythonNoImage
}

public enum CameraType
{
    frontCamera,
    bottomCamera
}

public enum ObjectType
{
    Big,
    Small,
    OnBottom,
    Manual
}

/// <summary>
/// Controls all academy
/// 
///     Setting up brain controll
///     Setting up object target
/// </summary>
public class RobotAcademy : Academy
{
    private static RobotAcademy mInstance;
    public static RobotAcademy Instance
    {
        get
        {

            return mInstance == null ? (mInstance = GameObject.Find("Academy").GetComponent<RobotAcademy>()) : mInstance;
        }
    }

    [Header("Controller settings")]
    public RobotControl control;
    public Brain learningBrain;
    public Brain learningBrainNoImage;
    public Brain playerBrain;
    public Brain padBrain;  

    /// <summary>
    /// The object creator
    /// </summary>
    [Header("Objects for creating datasets")]
    public Objects.ObjectCreator objectCreator = new Objects.ObjectCreator();

    [Header("Debug settings - use carefully!")]
    public bool forceDataCollection = false;
    public bool forceNoise = false;
    public bool forceNegativeExamples = false;

    [HideInInspector]
    public Vector2 VisualObservationResolution;
    private RobotAgent robotAgent;

    /// <summary>
    /// Called when validation.
    /// Setting up brain control.
    /// </summary>
    void OnValidate()
    {
        SetBrainControl();
    }

    void Start()
    {
        VisualObservationResolution.x = learningBrain.brainParameters.cameraResolutions[0].width;
        VisualObservationResolution.y = learningBrain.brainParameters.cameraResolutions[0].height;
        SetupAcademy();
    }

    void SetupAcademy()
    {
        SetBrainControl();
        if ((int)resetParameters["FocusedObject"] >= objectCreator.targetObjects.Count)
            resetParameters["FocusedObject"] = 0;
        SetFocusedObject((int)resetParameters["FocusedObject"]);
    }

    /// <summary>
    /// Specifies the academy behavior when being reset, setups academy to starting values.
    /// </summary>
    public override void AcademyReset()
    {
        SetupAcademy();
    }

    /// <summary>
    /// Initializes the academy and environment. Called during the waking-up
    /// phase of the environment before any of the scene objects/agents have
    /// been initialized. Setups the acdemy to starting values.
    /// </summary>
    public override void InitializeAcademy()
    {
        SetupAcademy();
    }

    public void SetFocusedObject(int index)
    {
        for (int i = 0; i < objectCreator.targetObjects.Count; i++)
        {
            objectCreator.targetIsEnabled[i] = false;
        }
        objectCreator.targetIsEnabled[index] = true;

        resetParameters["FocusedObject"] = index;
    }


    /// <summary>
    /// Setups brains the control.
    /// </summary>
    private void SetBrainControl()
    {
        if (control == RobotControl.player)
        {
            RobotAgent.Instance.GiveBrain(playerBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(playerBrain);
            RobotAgent.Instance.AgentSettings.collectObservations = false;
            RobotAgent.Instance.AgentSettings.targetReset = true;
        }
        else if (control == RobotControl.pad)
        {
            RobotAgent.Instance.GiveBrain(padBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(padBrain);
            broadcastHub.SetControlled(padBrain, true);
            RobotAgent.Instance.AgentSettings.collectObservations = false;
            RobotAgent.Instance.AgentSettings.targetReset = false;
        }
        else if (control == RobotControl.python)
        {
            RobotAgent.Instance.GiveBrain(learningBrain);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(learningBrain);
            broadcastHub.SetControlled(learningBrain, true);
            RobotAgent.Instance.AgentSettings.collectObservations = true;
            RobotAgent.Instance.AgentSettings.targetReset = false;
        }
        else if (control == RobotControl.pythonNoImage)
        {
            RobotAgent.Instance.GiveBrain(learningBrainNoImage);
            broadcastHub.broadcastingBrains.Clear();
            broadcastHub.broadcastingBrains.Add(learningBrainNoImage);
            broadcastHub.SetControlled(learningBrainNoImage, true);
            RobotAgent.Instance.AgentSettings.collectObservations = true;
            RobotAgent.Instance.AgentSettings.targetReset = false;
        }
    }
}
