using UnityEngine;
using MLAgents;
using System;
using System.Collections.Generic;
using Robot;
using Robot.Functionality;
using UnityEngine.Events;
using System.Text;

[System.Serializable]
public class AgentSettings
{
    public bool sendRelativeData = false;
    [ResetParameter("CollectData")]
    public bool dataCollection = false;
    [ResetParameter("Positive")]
    public bool positiveExamples = true;
    [ResetParameter]
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
    private TargetAnnotation annotation=null;

    [Header("Cameras")]
    public Camera frontCamera = null;
    public Camera bottomCamera = null;
    public Camera ActiveCamera { get; private set; } = null;

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

    VectorActions lastVectorAction = null;
    float lastReward = 0;
    
    public CameraType focusedCamera;

    bool initialized=false;
    void Initialize(){
        ResetParameterAttribute.InitializeAll(agentSettings);
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
        if (focusedCamera==CameraType.frontCamera)
        {
            agentParameters.agentCameras[0] = frontCamera;
            frontCamera.gameObject.SetActive(true);
            bottomCamera.gameObject.SetActive(false);
            ActiveCamera = frontCamera;
        }
        else if (focusedCamera==CameraType.bottomCamera)
        {
            agentParameters.agentCameras[0] = bottomCamera;
            frontCamera.gameObject.SetActive(false);
            bottomCamera.gameObject.SetActive(true);
            ActiveCamera = bottomCamera;
        }
        else
        {
            throw new InvalidEnumValueException(focusedCamera);
        }
    }

    void ApplyResetParameters(){
        agentParameters.maxStep = (int)RobotAcademy.Instance.GetResetParameter("AgentMaxSteps");
        focusedCamera = (CameraType)RobotAcademy.Instance.resetParameters["FocusedCamera"];
        SetCamera();
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

    public string GenerateDebugString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (lastVectorAction != null)
        {
            stringBuilder.Append("vector actions:\n");
            stringBuilder.Append(lastVectorAction.ToString());
        }
        stringBuilder.Append("\nobservations:\n");
        stringBuilder.Append(GetObservations().ToString());
        stringBuilder.Append("\nreward : ");
        stringBuilder.Append(lastReward);
        return stringBuilder.ToString();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        lastVectorAction = new VectorActions(vectorAction);
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
            engine.Move(lastVectorAction.longitudal, lastVectorAction.lateral, lastVectorAction.vertical, lastVectorAction.yaw);
            if (IsNewCameraChosen((CameraType)lastVectorAction.camera))
            {
                // focusedCamera = (CameraType)vectorAction[4];
                // SetCamera();
            }
            if (lastVectorAction.ball_grapper != 0)
            {
                ballGrapper.Grab();
            }
            if (lastVectorAction.torpedo != 0)
            {
                torpedo.Shoot();
            }
        }

        Target target = Targets.Focused;
        //Calculate target info for collecting data (in case of new position on each step) 
        if (agentSettings.randomizeTargetObjectPositionOnEachStep && target!=null)
        {
            targetCenter = Utils.GetComplexBounds(target.gameObject).center;
            targetRotation = target.transform.rotation;
        }

        //Collect data
        relativePosition = RelativeTargetPosition();
        relativeAngle = RelativeTargetAngle();
        float currentReward = CalculateReward();
        SetReward(currentReward);
        lastReward = currentReward;
    }

    public Vector3 RelativeTargetPosition(){
        Transform target = Targets.Focused?.transform;
        if (target == null){
            return Vector3.zero;
        }
        Vector3 distToCenter = target.InverseTransformPoint(targetCenter);
        Vector3 relativePos = target.InverseTransformPoint(transform.position) - distToCenter;
        relativePos.x = Mathf.Abs(relativePos.x);
        relativePos.y = Mathf.Abs(relativePos.y);
        relativePos.z = Mathf.Abs(relativePos.z);
        return relativePos;
    }

    public float RelativeTargetAngle()
    {
        Transform target = Targets.Focused?.transform;
        if (target==null){
            return 0f;
        }
        float relativeYaw = (Quaternion.Inverse(target.rotation) * transform.rotation).eulerAngles.y;
        relativeYaw = Mathf.Abs((relativeYaw + 180) % 360 - 180);
        return relativeYaw;
    }

    public Observations GetObservations()
    {
        Observations observations=new Observations();

        observations.acceleration = accelerometer.GetAcceleration();
        observations.angular_acceleration = accelerometer.GetAngularAcceleration();
        observations.rotation = accelerometer.GetRotation();
        observations.depth = depthSensor.GetDepth();

        if (agentSettings.dataCollection && agentSettings.positiveExamples)
            observations.bounding_box= annotation.GetBoundingBox();
        if (agentSettings.positiveExamples && !agentSettings.forceToSaveAsNegative)
            observations.positive_negative = 1.0f;

        if (agentSettings.sendRelativeData)
            observations.relative_position=new float[]{
                relativePosition.x,
                relativePosition.y,
                relativePosition.z,
                relativeAngle
            };

        observations.grab = (int)ballGrapper.GetState();
        observations.torpedo = torpedo.IsHit() ? 1 : 0;

        return observations;
    }

    public override void CollectObservations()
    {
        if (!agentSettings.collectObservations)
            return;
        
        AddVectorObs(GetObservations().array);
    }
    #endregion

    bool IsNewCameraChosen(CameraType newMode)
    {
        return newMode != focusedCamera;
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