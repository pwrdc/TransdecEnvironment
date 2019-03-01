using UnityEngine;
using MLAgents;
using System;

public class RobotAgent : Agent {

    [Header("Reward function settings")]
    public GameObject target;
    public Vector3 targetOffset = Vector3.zero;

    [Header("Additional settings")]
    public bool sendRelativeData = false;
    public bool dataCollection = false;
    public bool addNoise = false;
    public bool positiveExamples = true;
    public bool playerSteering = false;
    public RobotAcademy.DataCollection mode;
    public GameObject gateTargetObject;
    public GameObject pathTargetObject;

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

    void OnValidate() {
        GameObject agent = transform.parent.gameObject;
        annotations = agent.GetComponent<TargetAnnotation>();
        initializer = agent.GetComponent<RandomInit>();
        positionDrawer = agent.GetComponent<RandomPosition>();
        positionDrawer.agent = transform.gameObject;
        if (mode == RobotAcademy.DataCollection.gate){
            annotations.target = gateTargetObject;
            positionDrawer.target = gateTargetObject;
        }
        else if (mode == RobotAcademy.DataCollection.path){
            annotations.target = pathTargetObject;
            positionDrawer.target = pathTargetObject;
        }
    }

	void Start () {
        rbody = GetComponent<Rigidbody>();
        engine = transform.Find("Engine").GetComponent<Engine>();
        accelerometer = transform.Find("Accelerometer").GetComponent<Accelerometer>();
        depthSensor = transform.Find("DepthSensor").GetComponent<DepthSensor>();
        if (dataCollection)
            annotations.activate = true;
        if (positiveExamples)
            target.SetActive(true);
        else
            target.SetActive(false);
    }

    public override void AgentReset() {
        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        initializer.PutAll();
        targetCenter = GetComplexBounds(target).center;
        targetRotation = target.GetComponent<Rigidbody>().rotation;
        startPos = GetPosition();
        startAngle = GetAngle();
        SetReward(0);
    }

    public override void CollectObservations() {
        float[] toSend = new float[20];
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
        if (dataCollection)
            annotations.GetBoundingBox().CopyTo(toSend, toSendCell);
        // positive/negative example
        toSendCell += 4;
        if (positiveExamples)
            toSend[toSendCell] = 1.0f;
        else
            toSend[toSendCell] = 0.0f;
        // relative position data
        toSendCell += 1;
        if (sendRelativeData){
            toSend[toSendCell + 1] = pos.x;
            toSend[toSendCell + 2] = pos.y;
            toSend[toSendCell + 3] = pos.z;
            toSend[toSendCell + 4] = angle;
        }
        AddVectorObs(toSend);
    }

    public override void AgentAction(float[] vectorAction, string textAction){
        if (dataCollection)
            positionDrawer.DrawPositions(addNoise);
        else
            engine.Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
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
        if (other.gameObject.name == "TargetPlane" && !playerSteering)
            Done();
    }
}
