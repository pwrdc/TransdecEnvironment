using UnityEngine;
using MLAgents;
using System;

public class RobotAgent : Agent {
    public bool dataCollection = true;

    [Header("Reward function settings")]
    public GameObject target;
    public Vector3 targetOffset = Vector3.zero;

    Rigidbody rbody;
    Engine engine;
    Accelerometer accelerometer;
    DepthSensor depthSensor;
    Vector3 targetCenter;
    Quaternion targetRotation;
    Vector3 startPos;
    float startAngle;
    int collided = 0;

	void Start () {
        rbody = GetComponent<Rigidbody>();
        engine = transform.Find("Engine").GetComponent<Engine>();
        accelerometer = transform.Find("Accelerometer").GetComponent<Accelerometer>();
        depthSensor = transform.Find("DepthSensor").GetComponent<DepthSensor>();
        targetCenter = getTargetCenter();
        targetRotation = target.GetComponent<Rigidbody>().rotation;
	}

    public override void AgentReset() {
        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        transform.parent.GetComponent<RandomInit>().PutAll();
        if (dataCollection) {
            GameObject.Find("Robot").GetComponent<WaterOpacity>().dataCollecting = true;
            GameObject.Find("Robot").GetComponent<WaterOpacity>().SetUnderwater();
        }
        startPos = getPosition();
        startAngle = getAngle();
        SetReward(0);
    }

    public override void CollectObservations() {
        if (dataCollection) {
            float[] coords = GameObject.Find("Academy").GetComponent<TargetAnnotation>().boxCoord;
            AddVectorObs(coords);
        }
        else {
            float[] toSend = new float[10];
            float[] acceleration = accelerometer.getAcceleration();
            float[] angularAcceleration = accelerometer.getAngularAcceleration();
            float[] rotation = accelerometer.getRotation();
            acceleration.CopyTo(toSend, 0);
            angularAcceleration.CopyTo(toSend, acceleration.Length);
            rotation.CopyTo(toSend, acceleration.Length + angularAcceleration.Length);
            toSend[toSend.Length - 1] = depthSensor.depth;
            AddVectorObs(toSend);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction){
        engine.Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
        float currentReward = getReward();
        SetReward(currentReward);
    }

    Vector3 getTargetCenter() {
        Bounds bounds = new Bounds (target.transform.position, Vector3.zero);
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds.center;
    }

    Vector3 getPosition() {
        // relative position
        Vector3 distToCenter = target.transform.InverseTransformPoint(targetCenter);
        Vector3 relativePos = target.transform.InverseTransformPoint(rbody.position) - distToCenter - targetOffset;
        relativePos.x = Math.Abs(relativePos.x);
        relativePos.y = Math.Abs(relativePos.y);
        relativePos.z = Math.Abs(relativePos.z);
        return relativePos;
    }

    float getAngle() {
        // relative angle
        float relativeYaw = (Quaternion.Inverse(targetRotation) * rbody.rotation).eulerAngles.y;
        relativeYaw = Math.Abs((relativeYaw + 180) % 360 - 180);
        return relativeYaw;
    }

    float getReward() {
        /*  reward function, which is a normalized sum of expressions:
                -sqrt(-a_0 * a) + 1 
            calculated for each essential value
        */
        Vector3 pos = getPosition();
        float angle = getAngle();
        float reward = (calculateSingleReward(pos.x, startPos.x) + 
                        calculateSingleReward(pos.y, startPos.y) + 
                        calculateSingleReward(pos.z, startPos.z) +
                        calculateSingleReward(angle, startAngle)) / 4 -
                        collided - engine.isAboveSurface();   
        return reward;
    }

    float calculateSingleReward(float current, float start) {
        return (float)(-Math.Sqrt(1 / start * current) + 1);
    }

    void OnCollisionEnter() {
        collided = 1;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "TargetPlane")
            Done();
    }
}
