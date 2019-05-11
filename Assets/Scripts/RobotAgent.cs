using UnityEngine;
using MLAgents;
using System;
using System.Collections.Generic;

public class RobotAgent : Agent {

    [Header("Reward function settings")]
    public GameObject target;
    public GameObject targetAnnotation;
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
    [Range(0.1f, 0.3f)]
    public float minWaterFog = 0.2f;
    [Range(0.3f, 0.5f)]
    public float maxWaterFog = 0.4f;

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
    public RobotAcademy.DataCollection mode;
    public GameObject[] gateTargetObject = new GameObject[2];
    public GameObject[] pathTargetObject = new GameObject[2];
    public GameObject[] bouy_1_sideTargetObject = new GameObject[2];
    public GameObject noise = null;
    public bool randomQuarter = true;
    public bool randomPosition = true;
    public bool randomOrientation = true;

    [HideInInspector]
    public int focusedCamera = 0;
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

    void OnValidate() {
        GameObject agent = transform.parent.gameObject;
        annotations = agent.GetComponent<TargetAnnotation>();
        initializer = agent.GetComponent<RandomInit>();
        positionDrawer = agent.GetComponent<RandomPosition>();
        waterCurrent = agent.GetComponent<WaterCurrent>();
        positionDrawer.agent = transform.gameObject;
        waterCurrent.rbody = transform.gameObject.GetComponent<Rigidbody>();
         
        if (mode == RobotAcademy.DataCollection.gate){
            target = gateTargetObject[0];
            targetAnnotation = gateTargetObject[1];
            positionDrawer.mode = positionDrawer.gate;
        }
        else if (mode == RobotAcademy.DataCollection.path){
            target = pathTargetObject[0];
            targetAnnotation = pathTargetObject[1];
            positionDrawer.mode = positionDrawer.path;
        }
        else if (mode == RobotAcademy.DataCollection.buoy_1_side){
            target = bouy_1_sideTargetObject[0];
            targetAnnotation = bouy_1_sideTargetObject[1];
            positionDrawer.mode = positionDrawer.bouy_1;
        }

        annotations.target = targetAnnotation;
        positionDrawer.toAnnotate = targetAnnotation;
        positionDrawer.target = target;

    }

	void Start () {
        rbody = GetComponent<Rigidbody>();
        engine = transform.Find("Engine").GetComponent<Engine>();
        accelerometer = transform.Find("Accelerometer").GetComponent<Accelerometer>();
        depthSensor = transform.Find("DepthSensor").GetComponent<DepthSensor>();
        academy = GameObject.Find("Academy").GetComponent<RobotAcademy>();
        SetCamera();
        if(dataCollection) {
            tasks_objects.Clear();
            foreach (Transform child in transform.parent) {
                if (child.gameObject != target && child.gameObject != this.gameObject)
                    tasks_objects.Add(child.gameObject);
            }
            foreach(GameObject obj in tasks_objects) {
                obj.SetActive(false);
            }
        }
    }

    void OnApplicationQuit() {
        foreach(Transform child in transform.parent) {
            child.gameObject.SetActive(true);
        }
        noise.SetActive(false);
    }

    public override void AgentReset() {
        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        initializer.PutAll(randomQuarter, randomPosition, randomOrientation);
        initializer.LightInit(light, waterOpacity, minAngle, maxAngle, minIntensivity, maxIntensivity, minWaterFog, maxWaterFog);
        targetCenter = GetComplexBounds(target).center;
        targetRotation = target.GetComponent<Rigidbody>().rotation;
        startPos = GetPosition();
        startAngle = GetAngle();
        SetReward(0);
        if (dataCollection) {
            annotations.activate = true;
            agentParameters.numberOfActionsBetweenDecisions = 1;
        }
        target.SetActive(positiveExamples);
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
        if (dataCollection)
            positionDrawer.DrawPositions(addNoise, randomQuarter, randomPosition);
        else
            engine.Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);

        if (isCurrentEnabled)
            waterCurrent.AddCurrentToBoat();

        pos = GetPosition();
        angle = GetAngle();
        float currentReward = CalculateReward();
        SetReward(currentReward);
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
        if ((int)RobotAcademy.CameraID.frontCamera == focusedCamera) {
            agentParameters.agentCameras[0] = frontCamera;
            annotations.cam = frontCamera;
            frontCamera.targetDisplay = 0;
            bottomCamera.targetDisplay = 2;
        }
        else if ((int)RobotAcademy.CameraID.bottomCamera == focusedCamera) {
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
