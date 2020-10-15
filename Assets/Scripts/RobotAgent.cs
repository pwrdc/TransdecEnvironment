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
    public bool sendAllData = false;
    public bool targetReset = false;
    public bool collectObservations = false; //If agent is collecting data
    public bool randomizeTargetObjectPositionOnEachStep = true; //Only in data collection
}

public class RobotAgent : Agent
{
    static RobotAgent instance;
    public static RobotAgent Instance => Singleton.GetInstance(ref instance);

    public UnityEvent OnDataCollection;
    public UnityEvent OnReset;
    
    [SerializeField]
    private Hydrophone hydrophone = null;
    [SerializeField]
    private DistanceSensor depthSensor = null;
    [SerializeField]
    private DistanceSensor frontDistanceSensor = null;

    [Header("Cameras")]
    public Camera frontCamera = null;
    public Camera bottomCamera = null;
    public CameraType focusedCamera;
    public Camera ActiveCamera => (focusedCamera==CameraType.frontCamera) ? frontCamera : bottomCamera;

    private Engine engine;
    private Accelerometer accelerometer;
    private BallGrapper ballGrapper;
    private TorpedoLauncher torpedo;
    TargetSelector targetsSelector;
    TargetLocator targetLocator;
    bool selectingClosestTarget = false;

    public AgentSettings agentSettings = new AgentSettings();
    Rigidbody body;
    Vector3 targetCenter;
    Quaternion targetRotation;
    Vector3 startRelativePosition;
    float startRelativeAngle;
    int collided = 0;

    VectorAction lastVectorAction = null;
    string lastTextAction = "";
    Observations lastObservations = null;
    float lastReward = 0;

    bool initialized=false;
    void Initialize(){
        ResetParameterAttribute.InitializeAll(agentSettings);
        engine=GetComponentInChildren<Engine>();
        accelerometer=GetComponentInChildren<Accelerometer>();
        ballGrapper=GetComponentInChildren<BallGrapper>();
        torpedo=GetComponentInChildren<TorpedoLauncher>();
        body = GetComponent<Rigidbody>();
        targetsSelector = GetComponent<TargetSelector>();
        targetLocator = GetComponent<TargetLocator>();
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
            frontCamera.depth = 0;
            bottomCamera.depth = -1;
        }
        else if (focusedCamera==CameraType.bottomCamera)
        {
            agentParameters.agentCameras[0] = bottomCamera;
            frontCamera.depth = -1;
            bottomCamera.depth = 0;
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

        targetLocator.UpdateValues();
        startRelativePosition = targetLocator.RelativePosition;
        startRelativeAngle = targetLocator.RelativeAngle;

        //Reset reward
        SetReward(0);
        if (agentSettings.dataCollection)
        {
            agentParameters.numberOfActionsBetweenDecisions = 1;
        }
        selectingClosestTarget = false;
        OnReset.Invoke();
    }

    public string GenerateDebugString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (lastVectorAction != null)
        {
            stringBuilder.Append("vector actions:\n");
            stringBuilder.Append(lastVectorAction.ToString());
            stringBuilder.Append("\n");
        }
        stringBuilder.Append("text action: ");
        stringBuilder.Append(lastTextAction);
        stringBuilder.Append("\n\n");
        if (lastObservations != null)
        {
            stringBuilder.Append("observations:\n");
            stringBuilder.Append(lastObservations.ToString());
        }
        stringBuilder.Append("\nreward : ");
        stringBuilder.Append(lastReward);
        return stringBuilder.ToString();
    }

    void ApplyVectorAction(VectorAction vectorAction)
    {
        engine.Move(vectorAction.Longitudinal, vectorAction.Lateral, vectorAction.Vertical, vectorAction.Yaw);
        if ((CameraType)vectorAction.Camera != focusedCamera)
        {
            focusedCamera = (CameraType)vectorAction.Camera;
            SetCamera();
        }
        if (vectorAction.Grabber != 0)
            ballGrapper.Grab();
        if (vectorAction.Torpedo != 0)
            torpedo.Shoot();
        if (vectorAction.MarkerDropper != 0)
            EventsLogger.Log("Marker dropped");
        if(vectorAction.HydrophoneFrequency!=0)
            hydrophone.SetFrequency(vectorAction.HydrophoneFrequency);
        lastVectorAction = vectorAction;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (!initialized)
        {
            Initialize();
        }
        if (textAction != null && textAction!="")
        {
            /* Here the python script requests to fake object detection
               using in-Unity bounding box calculations.
               It is important to switch target to the closest one 
               because the robot can also use hydrophones,
               so they must both refer to the same (closest) object. */
            EventsLogger.Log($"Started faking object detection for target \"{textAction}\".");
            targetsSelector.ChangeTarget(textAction);
            selectingClosestTarget = true;
            lastTextAction = textAction;
        } else
        {
            lastTextAction = "";
        }

        if (agentSettings.dataCollection) //Collecting data
        {
            OnDataCollection.Invoke();
        }
        else // Testing/Training software 
        {
            ApplyVectorAction(new VectorAction(vectorAction));
        }

        Target target = Targets.Focused;
        // Calculate target info for collecting data (in case of new position on each step) 
        if (agentSettings.randomizeTargetObjectPositionOnEachStep && target!=null)
        {
            targetCenter = Utils.GetComplexBounds(target.gameObject).center;
            targetRotation = target.transform.rotation;
        }
        float currentReward = CalculateReward();
        SetReward(currentReward);
        lastReward = currentReward;
    }

    float[] EncodeBoundingBox(Rect boundingBox)
    {
        // sent bounding box needs to be flipped vertically
        return new float[]
        {
            // min point:
            boundingBox.xMin,
            1-boundingBox.yMax,
            // max point:
            boundingBox.xMax,
            1-boundingBox.yMin
        };
    }

    public Observations GetObservations()
    {
        Observations observations=new Observations();

        observations.Acceleration = accelerometer.acceleration.ToArray();
        observations.AngularAcceleration = accelerometer.angularAcceleration.ToArray();
        observations.Rotation = accelerometer.rotation.ToArray();
        observations.Depth = depthSensor.GetDistance();
        observations.FrontDistance = frontDistanceSensor.GetDistance();

        if(selectingClosestTarget)
            targetsSelector.SwitchToClosestTarget();
        targetLocator.UpdateValues();
        if (((agentSettings.dataCollection && agentSettings.positiveExamples) || agentSettings.sendAllData) && targetLocator.Visible)
            observations.BoundingBox=EncodeBoundingBox(targetLocator.ScreenRect);
        if ((agentSettings.positiveExamples && !agentSettings.forceToSaveAsNegative) || agentSettings.sendAllData)
            observations.PositiveNegative = targetLocator.Visible.ToInt();
        observations.HydrophoneAngle = hydrophone.GetAngle();

        Vector3 relativePosition = targetLocator.RelativePosition;
        if (agentSettings.sendRelativeData || agentSettings.sendAllData)
            observations.RelativePosition=new float[]{
                relativePosition.x,
                relativePosition.y,
                relativePosition.z,
                targetLocator.RelativeAngle
            };

        observations.GrabbingState = (int)ballGrapper.GetState();
        observations.TorpedoHit = torpedo.lastTorpedoHit.ToInt();
        observations.TorpedoReady= torpedo.ready.ToInt();

        lastObservations = observations;
        return observations;
    }

    public override void CollectObservations()
    {
        Observations observations = GetObservations();

        if (agentSettings.collectObservations)
            AddVectorObs(observations.array);
    }
    #endregion

    /// <summary>
    /// reward function, which is a normalized sum of expressions:
    /// -sqrt(1 / a_0* a) + 1
    /// calculated for each essential value
    /// </summary>
    /// <returns>reward</returns>
    float CalculateReward()
    {
        targetLocator.UpdateValues();
        Vector3 relativePosition = targetLocator.RelativePosition;
        float reward = (CalculateSingleReward(relativePosition.x, startRelativePosition.x) +
                        CalculateSingleReward(relativePosition.y, startRelativePosition.y) +
                        CalculateSingleReward(relativePosition.z, startRelativePosition.z) +
                        CalculateSingleReward(targetLocator.RelativeAngle, startRelativeAngle)) / 4 -
                        collided - (WaterLevel.IsAbove(transform.position.y).ToInt());
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