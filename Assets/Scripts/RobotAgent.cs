using UnityEngine;
using MLAgents;

public class RobotAgent : Agent {
    public bool dataCollection = true;

    [Header("Reward function settings")]
    public GameObject target;
    public Vector3 targetOffset = Vector3.zero;

    Rigidbody rbody;
    Accelerometer accelerometer;
    DepthSensor depthSensor;
    Vector3 targetCenter;
    Quaternion targetRotation;

	void Start () {
        rbody = GetComponent<Rigidbody>();
        accelerometer = transform.Find("Accelerometer").GetComponent<Accelerometer>();
        depthSensor = transform.Find("DepthSensor").GetComponent<DepthSensor>();
        targetCenter = getTargetCenter();
        targetRotation = target.GetComponent<Rigidbody>().rotation;
	}

    public override void AgentReset() {
        if (dataCollection) {
            GameObject.Find("Academy").GetComponent<RandomInit>().PutAll();
            GameObject.Find("Robot").GetComponent<WaterOpacity>().dataCollecting = true;
            GameObject.Find("Robot").GetComponent<WaterOpacity>().SetUnderwater();
        }
        SetReward(1);
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
        transform.Find("Engine").GetComponent<Engine>().Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
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

    float getReward() {
        // relative position
        Vector3 distToCenter = target.transform.InverseTransformPoint(targetCenter);
        Vector3 relativePos = target.transform.InverseTransformPoint(rbody.position) - distToCenter - targetOffset;
        // relative angle
        float relativeYaw = (Quaternion.Inverse(targetRotation) * rbody.rotation).eulerAngles.y;
        relativeYaw = (relativeYaw + 180) % 360 - 180;
        Debug.Log(relativeYaw);
        return 1.0f;
    }

    void Update() {
        getReward();
    }
}
