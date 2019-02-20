using UnityEngine;
using MLAgents;

public class RobotAgent : Agent {
    public bool dataCollection = true;
    Rigidbody rbody;

	void Start () {
        rbody = GetComponent<Rigidbody>();
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
            float[] a = new float[4];
            AddVectorObs(a);
        }
    }
    public override void AgentAction(float[] vectorAction, string textAction){
        transform.Find("Engine").GetComponent<Engine>().Move(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);
    }
}
