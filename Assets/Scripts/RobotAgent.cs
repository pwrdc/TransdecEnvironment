using UnityEngine;
using MLAgents;
using System;
using System.Collections.Generic;
using Robot;
using Robot.Functionality;
using UnityEngine.Events;

[System.Serializable]
public class AgentSettings
{
    public bool sendRelativeData = false;
    public bool dataCollection = false;
    public bool positiveExamples = true;
    public bool forceToSaveAsNegative = true;
    public bool targetReset = false;
    public bool collectObservations = false; //If agent is collecting data
    public bool randomizeTargetObjectPositionOnEachStep = true; //Only in data collection
}


public class RobotAgent : Agent
{
    //Singleton
    private static RobotAgent mInstance = null;
    public static RobotAgent Instance => 
        mInstance == null ? (mInstance = FindObjectOfType<RobotAgent>()) : mInstance;

    public UnityEvent OnDataCollection;
    public UnityEvent OnReset;
    
    [SerializeField]
    private TargetAnnotation annotation;

    [Header("Cameras")]
    public Camera frontCamera = null;
    public Camera bottomCamera = null;
    private Camera activeCamera = null;
    public Camera ActiveCamera { get { return activeCamera; } }

    private Engine engine;
    private DepthSensor depthSensor;
    private Accelerometer accelerometer;
    private BallGrapper ballGrapper;
    private Torpedo torpedo;

    public AgentSettings agentSettings = new AgentSettings();
    Rigidbody body;
    Vector3 targetCenter;
    Quaternion targetRotation;
    Vector3 startPos;
    float startRelativeAngle;
    int collided = 0;

    float relativeAngle; //angle between robot and target
    Vector3 relativePosition; //position between robot and target
    
    bool initialized=false;
    void Initialize(){
        engine=GetComponentInChildren<Engine>();
        depthSensor=GetComponentInChildren<DepthSensor>();
        accelerometer=GetComponentInChildren<Accelerometer>();
        ballGrapper=GetComponentInChildren<BallGrapper>();
        torpedo=GetComponentInChildren<Torpedo>();
        body = GetComponent<Rigidbody>();
        RobotAcademy.Instance.onResetParametersChanged.AddListener(ApplyResetParameters);
        SetCamera();
        AgentReset();
        initialized = true;
    }

    void Awake()
    {
        Initialize();
    }

    void SetCamera()
    {
        if (CameraType.frontCamera == TargetSettings.Instance.cameraType)
        {
            agentParameters.agentCameras[0] = frontCamera;
            frontCamera.targetDisplay = 0;
            bottomCamera.targetDisplay = 2;
            activeCamera = frontCamera;
        }
        else if (CameraType.bottomCamera == TargetSettings.Instance.cameraType)
        {
            agentParameters.agentCameras[0] = bottomCamera;
            bottomCamera.targetDisplay = 0;
            frontCamera.targetDisplay = 2;
            activeCamera = bottomCamera;
        }
        else
        {
            throw new Exception("Wrong camera was chosen");
        }
    }

    void ApplyResetParameters(){
        agentParameters.maxStep = (int)RobotAcademy.Instance.GetResetParameter("AgentMaxSteps");

        agentSettings.dataCollection = RobotAcademy.Instance.IsResetParameterTrue("CollectData");
        agentSettings.positiveExamples = RobotAcademy.Instance.IsResetParameterTrue("Positive");
        agentSettings.forceToSaveAsNegative = RobotAcademy.Instance.IsResetParameterTrue("ForceToSaveAsNegative");
    }

    #region Agent overrided methods
    
    public override void AgentReset()
    {
        //Reset robot
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;

        startPos = RelativeTargetPosition();
        startRelativeAngle = RelativeTargetAngle();

        //Reset reward
        SetReward(0);
        if (agentSettings.dataCollection)
        {
            agentParameters.numberOfActionsBetweenDecisions = 1;
        }
        OnReset.Invoke();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (!initialized)
        {
            Initialize();
        }

        if (agentSettings.dataCollection) //Collecting data
        {
            OnDataCollection.Invoke();
        }
        else //Testing/Training software 
        {
            engine.Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
            if (IsNewCameraChosen((CameraType)vectorAction[4]))
            {
                TargetSettings.Instance.cameraType = (CameraType)vectorAction[4];
                SetCamera();
            }
            if (vectorAction[5] == 1)
            {
                ballGrapper.Grab();
            }
            if (vectorAction[6] == 1)
            {
                torpedo.Shoot();
            }
        }

        //Calculate target info for collecting data (in case of new position on each step) 
        if (agentSettings.randomizeTargetObjectPositionOnEachStep)
        {
            targetCenter = Utils.GetComplexBounds(TargetSettings.Instance.target).center;
            targetRotation = TargetSettings.Instance.target.transform.rotation;
        }

        //Collect data
        relativePosition = RelativeTargetPosition();
        relativeAngle = RelativeTargetAngle();
        float currentReward = CalculateReward();
        SetReward(currentReward);
    }

    public Vector3 RelativeTargetPosition(){
        if(TargetSettings.Instance.target==null){
            return Vector3.zero;
        }
        Transform target=TargetSettings.Instance.target.transform;
        Vector3 targetOffset=TargetSettings.Instance.targetOffset;
        Vector3 distToCenter = target.InverseTransformPoint(targetCenter);
        Vector3 relativePos = target.InverseTransformPoint(transform.position) - distToCenter - targetOffset;
        relativePos.x = Mathf.Abs(relativePos.x);
        relativePos.y = Mathf.Abs(relativePos.y);
        relativePos.z = Mathf.Abs(relativePos.z);
        return relativePos;
    }

    public float RelativeTargetAngle(){
        if(TargetSettings.Instance.target==null){
            return 0f;
        }
        Transform target=TargetSettings.Instance.target.transform;
        float relativeYaw = (Quaternion.Inverse(target.rotation) * transform.rotation).eulerAngles.y;
        relativeYaw = Mathf.Abs((relativeYaw + 180) % 360 - 180);
        return relativeYaw;
    }

    public override void CollectObservations()
    {
        if (!agentSettings.collectObservations)
            return;

        float[] toSend = new float[21];
        float[] acceleration = accelerometer.GetAcceleration();
        float[] angularAcceleration = accelerometer.GetAngularAcceleration();
        float[] rotation = accelerometer.GetRotation();
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
        toSend[toSendCell] = depthSensor.GetDepth();
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
        //Grab state
        toSendCell += 4;
        toSend[toSendCell] = (int)ballGrapper.GetState();
        //Torpedo hit
        toSendCell += 1;
        toSend[toSendCell] = torpedo.IsHit() == true ? 1 : 0;
        
        AddVectorObs(toSend);
    }
    #endregion

    bool IsNewCameraChosen(CameraType newMode)
    {
        return newMode != TargetSettings.Instance.cameraType;
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
                        collided - (engine.isAboveSurface()?1:0);
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

    void OnCollisionEnter()
    {
        collided = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TargetPlane" && agentSettings.targetReset)
            Done();
    }
}