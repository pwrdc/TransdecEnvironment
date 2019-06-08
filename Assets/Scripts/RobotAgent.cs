using UnityEngine;
using MLAgents;
using System;
using System.Collections.Generic;

public class RobotAgent : Agent {

    [Header("Reward function settings")]
    public GameObject target;
    public GameObject targetAnnotation;
    public int targetIndex;
    public RobotAcademy.DataCollection targetMode;
    public Vector3 targetOffset = Vector3.zero;

    [Header("Light settings")]
    public Light light = null;
    public WaterOpacity waterOpacity = null;
    public int minAngle = 30;
    public int maxAngle = 150;
    [Range(0.0f, 1f)]
    public float minIntensivity = 0.1f;
    [Range(0.5f, 1.5f)]
    public float maxIntensivity = 1f;

    [Header("Water settings")]
    [Range(0.0f, 0.3f)]
    public float minWaterFog = 0.2f;
    [Range(0.2f, 0.6f)]
    public float maxWaterFog = 0.4f;
    [Range(0f, 1f)]
    public float minWaterColorB = 0.5f;
    [Range(0f, 1f)]
    public float maxWaterColorB = 0.8f;

    [Header("Camera settings")]
    public Camera frontCamera = null;
    public Camera bottomCamera = null;

    [Header("Additional settings")]
    public bool sendRelativeData = false;
    public bool dataCollection = false;
    public bool addNoise = false;
    public bool positiveExamples = true;
    public bool targetReset = false;
    public bool collectObservations = false;
    public GameObject noise = null;
    public bool randomQuarter = true;
    public bool randomPosition = true;
    public bool randomOrientation = true;

    [Header("Background settings")]
    public bool isBackgroundImage = false;
    public GameObject frontCameraBackground = null;
    public GameObject bottomCameraBackground = null;
    public int frequencyOfImageBackground = 10;
    int numOfBackgroundImages = 1;


    [HideInInspector]
    public bool isCurrentEnabled = true;

    Rigidbody rbody;
    Engine engine;
    Accelerometer accelerometer;
    DepthSensor depthSensor;
    Vector3 targetCenter;
    Quaternion targetRotation;
    Vector3 startPos;
    float startAngle;
    int collided = 0;

    float angle;
    Vector3 pos;

    TargetAnnotation annotations;
    RandomInit initializer;
    RandomPosition positionDrawer;
    WaterCurrent waterCurrent;

    RobotAcademy academy;
    List<GameObject> tasks_objects = new List<GameObject>();

    bool isInitialized = false;

    int numberOfImageToDisplay = 0;
    int numberOfDisplayedImage = 0;

    void OnValidate() {
        SetAgent();
    }

    public void SetAgent() {
        Initialization();
        positionDrawer.agent = transform.gameObject;
        waterCurrent.rbody = transform.gameObject.GetComponent<Rigidbody>();

        annotations.target = targetAnnotation;
        positionDrawer.toAnnotate = targetAnnotation;
        positionDrawer.target = target;
        annotations.activatedMode = targetMode;
        annotations.activateBackground = isBackgroundImage;
        positionDrawer.ActivateOption(targetIndex);
        SetCamera();
    }

    void Initialization() {
        if(isInitialized)
            return;

        GameObject agent = transform.parent.gameObject;
        annotations = agent.GetComponent<TargetAnnotation>();
        initializer = agent.GetComponent<RandomInit>();
        positionDrawer = agent.GetComponent<RandomPosition>();
        waterCurrent = agent.GetComponent<WaterCurrent>();
        rbody = GetComponent<Rigidbody>();
        engine = transform.Find("Engine").GetComponent<Engine>();
        accelerometer = transform.Find("Accelerometer").GetComponent<Accelerometer>();
        depthSensor = transform.Find("DepthSensor").GetComponent<DepthSensor>();
        academy = GameObject.Find("Academy").GetComponent<RobotAcademy>();
    }

    void Awake() {
        Initialization();
    }

	void Start () {
        SetCamera();

        if(dataCollection) {
            ClearEnvironment();
            if(isBackgroundImage) 
                ActivateEnvironmentMeshRenderer(false);

            frontCameraBackground.SetActive(isBackgroundImage);
            bottomCameraBackground.SetActive(isBackgroundImage);
            annotations.activateBackground = isBackgroundImage;
            annotations.frontCameraBackground = frontCameraBackground;
            annotations.bottomCameraBackground = bottomCameraBackground;
            annotations.activatedMode = targetMode;
            numOfBackgroundImages = annotations.getNumberOfBackgroundImages();
        }
    }

    void ClearEnvironment() {
        tasks_objects.Clear();
        foreach(Transform child in transform.parent) {
            child.gameObject.SetActive(true);
        }
        foreach (Transform child in transform.parent) {
            if (child.gameObject != target && child.gameObject != this.gameObject)
                tasks_objects.Add(child.gameObject);
        }
        foreach(GameObject obj in tasks_objects) {
            obj.SetActive(false);
        }
    }

    void ActivateEnvironmentMeshRenderer(bool activate) {
        GameObject transdec = GameObject.Find("Transdec");
        MeshRenderer[] meshRenderers = transdec.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mesh in meshRenderers) {
            mesh.enabled = activate;
        }
    }

    void OnApplicationQuit() {
        foreach(Transform child in transform.parent) {
            child.gameObject.SetActive(true);
        }
        noise.SetActive(false);

        ActivateEnvironmentMeshRenderer(true);

        frontCameraBackground.SetActive(false);
        bottomCameraBackground.SetActive(false);
    }

    public override void AgentReset() {
        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        initializer.PutAll(randomQuarter, randomPosition, randomOrientation);
        initializer.EnvironmentInit(light, waterOpacity, minAngle, maxAngle, 
        					  minIntensivity, maxIntensivity, minWaterFog, maxWaterFog,
        					  minWaterColorB, maxWaterColorB);
        targetCenter = GetComplexBounds(target).center;
        targetRotation = target.transform.rotation;
        startPos = GetPosition();
        startAngle = GetAngle();
        SetReward(0);
        if (dataCollection) {
            annotations.activate = true;
            agentParameters.numberOfActionsBetweenDecisions = 1;
            ClearEnvironment(); 
            frontCameraBackground.SetActive(isBackgroundImage);
            bottomCameraBackground.SetActive(isBackgroundImage);
            ActivateEnvironmentMeshRenderer(!isBackgroundImage);
            target.SetActive(positiveExamples);
        }
        

        numberOfImageToDisplay = 0;
    }

    public override void CollectObservations() {
        if(!collectObservations)
            return;

        float[] toSend = new float[19];
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
        if (dataCollection && positiveExamples)
            annotations.GetBoundingBox().CopyTo(toSend, toSendCell);
        // positive/negative example
        toSendCell += 4;
        if (positiveExamples)
            toSend[toSendCell] = 1.0f;
        else
            toSend[toSendCell] = 0.0f;
        // relative position data
        if (sendRelativeData) {
            toSend[toSendCell + 1] = pos.x;
            toSend[toSendCell + 2] = pos.y;
            toSend[toSendCell + 3] = pos.z;
            toSend[toSendCell + 4] = angle;
        }
        AddVectorObs(toSend);
    }

    public override void AgentAction(float[] vectorAction, string textAction){
        if (dataCollection) {
            positionDrawer.DrawPositions(addNoise, randomQuarter, randomPosition);
	        
            initializer.EnvironmentInit(light, waterOpacity, minAngle, maxAngle, 
	        					  minIntensivity, maxIntensivity, minWaterFog, maxWaterFog,
	        					  minWaterColorB, maxWaterColorB);        
        }
        else {
            engine.Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
        }

        if (isCurrentEnabled)
            waterCurrent.AddCurrentToBoat();

        pos = GetPosition();
        angle = GetAngle();
        float currentReward = CalculateReward();
        SetReward(currentReward);

        if(isBackgroundImage) {
            numberOfDisplayedImage++;
            if(numberOfDisplayedImage > frequencyOfImageBackground) {
                annotations.ChangeImageToDisplay(numberOfImageToDisplay);
                numberOfImageToDisplay++;
                numberOfDisplayedImage = 0;
                if(numberOfImageToDisplay >= numOfBackgroundImages)
                    numberOfImageToDisplay = 0;
            }
        }
    }

    public Bounds GetComplexBounds(GameObject obj) {
        Bounds bounds = new Bounds (obj.transform.position, Vector3.zero);
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        
        return bounds;
    }

    void SetCamera() {
        //Show focused camera on Display 1 and set as agentCamera
        if (RobotAcademy.DataCollection.frontCamera == targetMode) {
            agentParameters.agentCameras[0] = frontCamera;
            annotations.cam = frontCamera;
            frontCamera.targetDisplay = 0;
            bottomCamera.targetDisplay = 2;
        }
        else if (RobotAcademy.DataCollection.bottomCamera == targetMode) {
            agentParameters.agentCameras[0] = bottomCamera;
            annotations.cam = bottomCamera;
            bottomCamera.targetDisplay = 0;
            frontCamera.targetDisplay = 2;
        }
        else {
            Debug.Log("ERROR");
            //TO DO: SHOW ERROR ON CHOOSING WRONG CAMERA
        }
    }

    Vector3 GetPosition() {
        // relative position
        Vector3 distToCenter = target.transform.InverseTransformPoint(targetCenter);
        Vector3 relativePos = target.transform.InverseTransformPoint(rbody.position) - distToCenter - targetOffset;
        relativePos.x = Math.Abs(relativePos.x);
        relativePos.y = Math.Abs(relativePos.y);
        relativePos.z = Math.Abs(relativePos.z);
        return relativePos;
    }

    float GetAngle() {
        // relative angle
        float relativeYaw = (Quaternion.Inverse(targetRotation) * rbody.rotation).eulerAngles.y;
        relativeYaw = Math.Abs((relativeYaw + 180) % 360 - 180);
        return relativeYaw;
    }

    float CalculateReward() {
        /*  reward function, which is a normalized sum of expressions:
                -sqrt(1 / a_0 * a) + 1 
            calculated for each essential value
        */
        float reward = (CalculateSingleReward(pos.x, startPos.x) + 
                        CalculateSingleReward(pos.y, startPos.y) + 
                        CalculateSingleReward(pos.z, startPos.z) +
                        CalculateSingleReward(angle, startAngle)) / 4 -
                        collided - engine.isAboveSurface();   
        return reward;
    }

    float CalculateSingleReward(float current, float start) {
        return (float)(-Math.Sqrt(1 / start * current) + 1);
    }

    void OnCollisionEnter() {
        collided = 1;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "TargetPlane" && targetReset)
            Done();
    }
}
