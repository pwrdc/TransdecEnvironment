// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="RobotAgent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using MLAgents;
using System;
using System.Collections.Generic;

/// <summary>
/// Class TargetSettings.
/// </summary>
[System.Serializable]
public class TargetSettings
{
    /// <summary>
    /// The target
    /// </summary>
    public GameObject target;
    /// <summary>
    /// The target annotation
    /// </summary>
    public GameObject targetAnnotation;
    /// <summary>
    /// The target index
    /// </summary>
    public int targetIndex;
    /// <summary>
    /// The camera type
    /// </summary>
    public CameraType cameraType;
    /// <summary>
    /// The target offset
    /// </summary>
    public Vector3 targetOffset = Vector3.zero;
}

/// <summary>
/// Class AgentSettings.
/// </summary>
[System.Serializable]
public class AgentSettings
{
    /// <summary>
    /// The send relative data
    /// </summary>
    public bool sendRelativeData = false;
    /// <summary>
    /// The data collection
    /// </summary>
    public bool dataCollection = false;
    /// <summary>
    /// The positive examples
    /// </summary>
    public bool positiveExamples = true;
    /// <summary>
    /// The force to save as negative
    /// </summary>
    public bool forceToSaveAsNegative = true;
    /// <summary>
    /// The target reset
    /// </summary>
    public bool targetReset = false;
    /// <summary>
    /// The collect observations
    /// </summary>
    public bool collectObservations = false; //If agent is collecting data
    /// <summary>
    /// The randomize target object position on each step
    /// </summary>
    public bool randomizeTargetObjectPositionOnEachStep = true; //Only in data collection
}


/// <summary>
/// Class RobotAgent.
/// Implements the <see cref="MLAgents.Agent" />
/// </summary>
/// <seealso cref="MLAgents.Agent" />
public class RobotAgent : Agent
{
    #region Fields and Properties
    //Singleton
    /// <summary>
    /// The m instance
    /// </summary>
    private static RobotAgent mInstance = null;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static RobotAgent Instance
    {
        get
        {
            return mInstance == null ? (mInstance = GameObject.Find("Agent").GetComponent<RobotAgent>()) : mInstance;
        }
    }

    //Events
    [HideInInspector]
    public event Action OnDataUpdate;
    [HideInInspector]
    public event Action<TargetSettings> OnDataTargetUpdate;
    [HideInInspector]
    public event Action<Objects.ObjectConfigurationSettings> OnDataConfigurationUpdate;
    [HideInInspector]
    public event Action<BackgroundSettings> OnDataBackgroundUpdate;
    [HideInInspector]
    public event Action<AgentSettings> OnDataAgentUpdate;
    [HideInInspector]
    public event Action<SceneEnvironment.EnvironmentSettings> OnDataEnvironmentUpdate;
    [HideInInspector]
    public event Action<SceneEnvironment.EnvironmentInitValues> OnDataEnvironmentValuesUpdate;

    /// <summary>
    /// The environment manager
    /// </summary>
    [Header("Managers")]
    [SerializeField]
    private SceneEnvironment.EnvironmentManager environmentManager;
    /// <summary>
    /// The robot
    /// </summary>
    [SerializeField]
    private Robot.Robot robot;
    /// <summary>
    /// Gets the robot.
    /// </summary>
    /// <value>The robot.</value>
    public Robot.Robot Robot { get { return robot; } }
    /// <summary>
    /// The object manager
    /// </summary>
    [SerializeField]
    private Objects.ObjectManager objectManager;
    /// <summary>
    /// The annotation
    /// </summary>
    [SerializeField]
    private TargetAnnotation annotation;
    /// <summary>
    /// The background manager
    /// </summary>
    [SerializeField]
    private BackgroundImageManager backgroundManager;

    /// <summary>
    /// The front camera
    /// </summary>
    [Header("Camera settings")]
    public Camera frontCamera = null;
    /// <summary>
    /// The bottom camera
    /// </summary>
    public Camera bottomCamera = null;
    /// <summary>
    /// The active camera
    /// </summary>
    private Camera activeCamera = null;
    /// <summary>
    /// Gets the active camera.
    /// </summary>
    /// <value>The active camera.</value>
    public Camera ActiveCamera { get { return activeCamera; } }

    /// <summary>
    /// The target settings
    /// </summary>
    [Header("Custom Settings")]
    [SerializeField]
    private TargetSettings targetSettings = new TargetSettings();
    /// <summary>
    /// The agent settings
    /// </summary>
    [SerializeField]
    private AgentSettings agentSettings = new AgentSettings();
    /// <summary>
    /// The object configuration settings
    /// </summary>
    [SerializeField]
    private Objects.ObjectConfigurationSettings objectConfigurationSettings = new Objects.ObjectConfigurationSettings();
    /// <summary>
    /// The background settings
    /// </summary>
    [SerializeField]
    private BackgroundSettings backgroundSettings = new BackgroundSettings();
    /// <summary>
    /// The environment settings
    /// </summary>
    [SerializeField]
    private SceneEnvironment.EnvironmentSettings environmentSettings = new SceneEnvironment.EnvironmentSettings();
    /// <summary>
    /// The environment values settings
    /// </summary>
    [SerializeField]
    private SceneEnvironment.EnvironmentInitValues environmentValuesSettings = new SceneEnvironment.EnvironmentInitValues();


    /// <summary>
    /// Gets the target settings.
    /// </summary>
    /// <value>The target settings.</value>
    public TargetSettings TargetSettings { get { return targetSettings; } }
    /// <summary>
    /// Gets the agent settings.
    /// </summary>
    /// <value>The agent settings.</value>
    public AgentSettings AgentSettings { get { return agentSettings; } }
    /// <summary>
    /// Gets the object configuration settings.
    /// </summary>
    /// <value>The object configuration settings.</value>
    public Objects.ObjectConfigurationSettings ObjectConfigurationSettings { get { return objectConfigurationSettings; } }
    /// <summary>
    /// Gets the background settings.
    /// </summary>
    /// <value>The background settings.</value>
    public BackgroundSettings BackgroundSettings { get { return backgroundSettings; } }
    /// <summary>
    /// Gets the environment settings.
    /// </summary>
    /// <value>The environment settings.</value>
    public SceneEnvironment.EnvironmentSettings EnvironmentSettings { get { return environmentSettings; } }
    /// <summary>
    /// Gets the environment initialize values.
    /// </summary>
    /// <value>The environment initialize values.</value>
    public SceneEnvironment.EnvironmentInitValues EnvironmentInitValues { get { return environmentValuesSettings; } }


    /// <summary>
    /// The robot rigidbody
    /// </summary>
    Rigidbody RobotRigidbody;
    /// <summary>
    /// The target center
    /// </summary>
    Vector3 targetCenter;
    /// <summary>
    /// The target rotation
    /// </summary>
    Quaternion targetRotation;
    /// <summary>
    /// The start position
    /// </summary>
    Vector3 startPos;
    /// <summary>
    /// The start relative angle
    /// </summary>
    float startRelativeAngle;
    /// <summary>
    /// The collided
    /// </summary>
    int collided = 0;

    /// <summary>
    /// The relative angle
    /// </summary>
    float relativeAngle; //angle between robot and target
    /// <summary>
    /// The relative position
    /// </summary>
    Vector3 relativePosition; //position between robot and target

    /// <summary>
    /// The tasks objects
    /// </summary>
    List<GameObject> tasksObjects = new List<GameObject>();

    /// <summary>
    /// The is initialized
    /// </summary>
    bool isInitialized = false;
    #endregion

    #region Setting up Agent
    void OnValidate()
    {
        SetAgent();
    }

    void Awake()
    {
        Initialization();
    }

    void Start()
    {
        SetAgent();
        ResetAgent();
    }

    /// <summary>
    /// Sets agent:
    /// Initalizes agent
    /// Setups information from robot academy
    /// Sets camera
    /// Invoke all events
    /// </summary>
    void SetAgent()
    {
        Initialization();
        SetupRobotAcademyInfo();
        SetCamera(); 
        InvokeAllEvents(); 
    }

    void Initialization()
    {
        if (isInitialized)
            return;
        isInitialized = true;

        RobotRigidbody = robot.gameObject.GetComponent<Rigidbody>();
        Utils.GetObjectsInFolder(objectConfigurationSettings.tasksFolder, out tasksObjects);
    }

    /// <summary>
    /// Show focused camera on Display 1 and set as agentCamera
    /// </summary>
    /// <exception cref="Exception">Wrong camera was choosen</exception>
    void SetCamera()
    {
        if (CameraType.frontCamera == targetSettings.cameraType)
        {
            agentParameters.agentCameras[0] = frontCamera;
            frontCamera.targetDisplay = 0;
            bottomCamera.targetDisplay = 2;
            activeCamera = frontCamera;
        }
        else if (CameraType.bottomCamera == targetSettings.cameraType)
        {
            agentParameters.agentCameras[0] = bottomCamera;
            bottomCamera.targetDisplay = 0;
            frontCamera.targetDisplay = 2;
            activeCamera = bottomCamera;
        }
        else
        {
            Debug.LogError("Chosed wrong camera");
            throw new Exception("Wrong camera was choosen");
        }
    }

    /// <summary>
    /// Setups robot academy info for agent
    /// </summary>
    void SetupRobotAcademyInfo()
    {
        agentParameters.maxStep = (int)RobotAcademy.Instance.resetParameters["AgentMaxSteps"];

        agentSettings.dataCollection = RobotAcademy.Instance.resetParameters["CollectData"] == 0 ? false : true;
        agentSettings.positiveExamples = RobotAcademy.Instance.resetParameters["Positive"] == 0 ? false : true;
        agentSettings.forceToSaveAsNegative = RobotAcademy.Instance.resetParameters["ForceToSaveAsNegative"] == 0 ? false : true;

        targetSettings.cameraType = (CameraType)RobotAcademy.Instance.resetParameters["FocusedCamera"];
        targetSettings.targetIndex = (int)RobotAcademy.Instance.resetParameters["FocusedObject"];
        targetSettings.target = RobotAcademy.Instance.objectCreator.targetObjects[targetSettings.targetIndex];
        targetSettings.targetAnnotation = RobotAcademy.Instance.objectCreator.targetAnnotations[targetSettings.targetIndex];

        backgroundSettings.isBackgroundImage = RobotAcademy.Instance.resetParameters["EnableBackgroundImage"] == 0 ? false : true;
        

        objectConfigurationSettings.addNoise = RobotAcademy.Instance.resetParameters["EnableNoise"] == 0 ? false : true;
        objectConfigurationSettings.setFocusedObjectInCenter = RobotAcademy.Instance.resetParameters["SetFocusedObjectInCenter"] == 0 ? false : true;

        environmentSettings.isCurrentEnabled = RobotAcademy.Instance.resetParameters["WaterCurrent"] == 0 ? false : true;
    }
    #endregion

    /// <summary>
    /// Invoke all events
    /// </summary>
    void InvokeAllEvents()
    {
        if (OnDataUpdate != null)
            OnDataUpdate.Invoke();

        if (OnDataTargetUpdate != null)
            OnDataTargetUpdate.Invoke(targetSettings);

        if (OnDataConfigurationUpdate != null)
            OnDataConfigurationUpdate.Invoke(objectConfigurationSettings);

        if (OnDataBackgroundUpdate != null)
            OnDataBackgroundUpdate.Invoke(backgroundSettings);

        if (OnDataAgentUpdate != null)
            OnDataAgentUpdate.Invoke(agentSettings);

        if (OnDataEnvironmentUpdate != null)
            OnDataEnvironmentUpdate.Invoke(environmentSettings);

        if (OnDataEnvironmentValuesUpdate != null)
            OnDataEnvironmentValuesUpdate.Invoke(environmentValuesSettings);
    }

    /// <summary>
    /// Clear all tasks that are not active (only for collecting data)
    /// </summary>
    void DisableAllInactiveTasks()
    {
        foreach (var obj in tasksObjects)
        {
            if (obj != targetSettings.target)
                obj.SetActive(false);
        }
    }

    /// <summary>
    /// Set environment to normal
    /// Tasks are active
    /// Background is disabled
    /// Transdec is active
    /// Noise is disabled
    /// </summary>
    void OnApplicationQuit()
    {

        foreach (var obj in tasksObjects)
        {
            obj.SetActive(true);
        }

        objectConfigurationSettings.noiseFolder.SetActive(false);
        backgroundManager.EnableBackgroundImage(false);
    }

    #region Agent overrided methods
    /// <summary>
    /// Specifies the agent behavior when being reset, which can be due to
    /// the agent or Academy being done (i.e. completion of local or global
    /// episode).
    /// </summary>
    public override void AgentReset()
    {
        ResetAgent();
    }

    /// <summary>
    /// Specifies the agent behavior at every step based on the provided
    /// action.
    ///     Setups environment, then target object
    /// </summary>
    /// <param name="vectorAction">Vector action. Note that for discrete actions, the provided array
    /// will be of length 1.</param>
    /// <param name="textAction">Text action.</param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (agentSettings.dataCollection) //Collecting data
        {
            //Randomize environment (Water color and light)
            if (environmentSettings.isEnvironmentRandomized && environmentSettings.isEnvironmentInitOnEachStep)
                environmentManager.EnvironmentRandomizedInit();

            //Randomize target object position
            if (agentSettings.randomizeTargetObjectPositionOnEachStep)  
                objectManager.RandomizeTargetPosition();

            //Randomize camera position
            objectManager.RandomizeCameraPositionFocusedOnTarget();

            //Set background
            if (backgroundSettings.isBackgroundImage) 
                backgroundManager.SetNewBackground();

        }
        else //Testing/Training software 
        {
            robot.Engine.Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
            if (IsNewCameraChosed((CameraType)vectorAction[4]))
            {
                targetSettings.cameraType = (CameraType)vectorAction[4];
                SetAgent();
            }
        }


        //Calculate target info for collecting data (in case of new position on each step) 
        if (agentSettings.randomizeTargetObjectPositionOnEachStep)
        {
            targetCenter = Utils.GetComplexBounds(targetSettings.target).center;
            targetRotation = targetSettings.target.transform.rotation;
        }

        //Collect data
        relativePosition = GetPosition();
        relativeAngle = GetAngle();
        float currentReward = CalculateReward();
        SetReward(currentReward);
    }

    /// <summary>
    /// Collects the (vector, visual, text) observations of the agent.
    /// The agent observation describes the current environment from the
    /// perspective of the agent.
    /// </summary>
    /// <remarks>Simply, an agents observation is any environment information that helps
    /// the Agent acheive its goal. For example, for a fighting Agent, its
    /// observation could include distances to friends or enemies, or the
    /// current level of ammunition at its disposal.
    /// Recall that an Agent may attach vector, visual or textual observations.
    /// Vector observations are added by calling the provided helper methods:
    /// - <see cref="AddVectorObs(int)" />
    /// - <see cref="AddVectorObs(float)" />
    /// - <see cref="AddVectorObs(Vector3)" />
    /// - <see cref="AddVectorObs(Vector2)" />
    /// - <see><cref>AddVectorObs(float[])</cref></see>
    /// - <see><cref>AddVectorObs(List{float})</cref></see>
    /// - <see cref="AddVectorObs(Quaternion)" />
    /// - <see cref="AddVectorObs(bool)" />
    /// - <see cref="AddVectorObs(int, int)" />
    /// Depending on your environment, any combination of these helpers can
    /// be used. They just need to be used in the exact same order each time
    /// this method is called and the resulting size of the vector observation
    /// needs to match the vectorObservationSize attribute of the linked Brain.
    /// Visual observations are implicitly added from the cameras attached to
    /// the Agent.
    /// Lastly, textual observations are added using
    /// <see cref="SetTextObs(string)" />.</remarks>
    public override void CollectObservations()
    {
        if (!agentSettings.collectObservations)
            return;

        float[] toSend = new float[19];
        float[] acceleration = robot.Accelerometer.GetAcceleration();
        float[] angularAcceleration = robot.Accelerometer.GetAngularAcceleration();
        float[] rotation = robot.Accelerometer.GetRotation();
        // acceleration data
        int toSendCell = 0;
        acceleration.CopyTo(toSend, toSendCell);
        // angular acceleration data
        toSendCell += acceleration.Length;
        angularAcceleration.CopyTo(toSend, toSendCell);
        // rotation data
        toSendCell += angularAcceleration.Length;
        rotation.CopyTo(toSend, toSendCell);
        // depth data
        toSendCell += rotation.Length;
        toSend[toSendCell] = robot.DepthSensor.GetDepth();
        // bounding box
        toSendCell += 1;
        if (agentSettings.dataCollection && agentSettings.positiveExamples)
            annotation.GetBoundingBox().CopyTo(toSend, toSendCell);
        // positive/negative example
        toSendCell += 4;
        if (agentSettings.positiveExamples && !agentSettings.forceToSaveAsNegative)
            toSend[toSendCell] = 1.0f;
        else
            toSend[toSendCell] = 0.0f;
        // relative position data
        if (agentSettings.sendRelativeData)
        {
            toSend[toSendCell + 1] = relativePosition.x;
            toSend[toSendCell + 2] = relativePosition.y;
            toSend[toSendCell + 3] = relativePosition.z;
            toSend[toSendCell + 4] = relativeAngle;
        }
        AddVectorObs(toSend);
    }
    #endregion

    /// <summary>
    /// Resets the agent.
    /// </summary>
    void ResetAgent()
    {
        //Reset robot
        this.RobotRigidbody.angularVelocity = Vector3.zero;
        this.RobotRigidbody.velocity = Vector3.zero;

        startPos = GetPosition();
        startRelativeAngle = GetAngle();

        //Reset reward
        SetReward(0);

        //Reset scene
        if (environmentSettings.isEnvironmentRandomized)
            environmentManager.EnvironmentRandomizedInit();
        else
            environmentManager.EnvironmentNormalInit();

        if (agentSettings.dataCollection)
        {
            agentParameters.numberOfActionsBetweenDecisions = 1;
            //Set only target task visibled
            DisableAllInactiveTasks();

            //Set Noise enabled/disabled
            objectConfigurationSettings.noiseFolder.SetActive(objectConfigurationSettings.addNoise);

            //Set background enabled/disabled
            backgroundManager.EnableBackgroundImage(backgroundSettings.isBackgroundImage);

            //Set target enabled/disabled
            targetSettings.target.SetActive(agentSettings.positiveExamples);
        }
    }

    /// <summary>
    /// Determines whether [is new camera chosed] [the specified new mode].
    /// </summary>
    /// <param name="newMode">The new mode.</param>
    /// <returns><c>true</c> if [is new camera chosed] [the specified new mode]; otherwise, <c>false</c>.</returns>
    bool IsNewCameraChosed(CameraType newMode)
    {
        return newMode == targetSettings.cameraType ? false : true;
    }

    /// <summary>
    /// Relative position between robot and target
    /// </summary>
    /// <returns>relative position</returns>
    Vector3 GetPosition()
    {
        Vector3 distToCenter = targetSettings.target.transform.InverseTransformPoint(targetCenter);
        Vector3 relativePos = targetSettings.target.transform.InverseTransformPoint(RobotRigidbody.position) - distToCenter - targetSettings.targetOffset;
        relativePos.x = Math.Abs(relativePos.x);
        relativePos.y = Math.Abs(relativePos.y);
        relativePos.z = Math.Abs(relativePos.z);
        return relativePos;
    }

    /// <summary>
    /// Get angle between robot and target
    /// </summary>
    /// <returns>relative angle</returns>
    float GetAngle()
    {
        float relativeYaw = (Quaternion.Inverse(targetRotation) * RobotRigidbody.rotation).eulerAngles.y;
        relativeYaw = Math.Abs((relativeYaw + 180) % 360 - 180);
        return relativeYaw;
    }

    /// <summary>
    /// reward function, which is a normalized sum of expressions:
    /// -sqrt(1 / a_0* a) + 1
    /// calculated for each essential value
    /// </summary>
    /// <returns>reward</returns>
    float CalculateReward()
    {
        float reward = (CalculateSingleReward(relativePosition.x, startPos.x) +
                        CalculateSingleReward(relativePosition.y, startPos.y) +
                        CalculateSingleReward(relativePosition.z, startPos.z) +
                        CalculateSingleReward(relativeAngle, startRelativeAngle)) / 4 -
                        collided - robot.Engine.isAboveSurface();
        return reward;
    }

    /// <summary>
    /// Calculates the single reward.
    /// </summary>
    /// <param name="current">The current.</param>
    /// <param name="start">The start.</param>
    /// <returns>System.Single.</returns>
    float CalculateSingleReward(float current, float start)
    {
        return (float)(-Math.Sqrt(1 / start * current) + 1);
    }

    /// <summary>
    /// Called when [collision enter].
    /// </summary>
    void OnCollisionEnter()
    {
        collided = 1;
    }

    /// <summary>
    /// Called when [trigger enter].
    /// </summary>
    /// <param name="other">The other.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TargetPlane" && agentSettings.targetReset)
            Done();
    }
}
