using UnityEngine;
using MLAgents;

public class RobotAgent : Agent {
    public bool dataCollection = true;
    Rigidbody rbody;
    Accelerometer accelerometer;
    DepthSensor depthSensor;

	void Start () {
        rbody = GetComponent<Rigidbody>();
        accelerometer = transform.Find("Accelerometer").GetComponent<Accelerometer>();
        depthSensor = transform.Find("DepthSensor").GetComponent<DepthSensor>();
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
}
