using UnityEngine;
using MLAgents;

public class RobotAgent : Agent {
    Rigidbody rbody;

    float a = 0.0f;

	void Start () {
        rbody = GetComponent<Rigidbody>();
	}

    public override void AgentReset() {
        GameObject.Find("Academy").GetComponent<RandomInit>().PutAll();
    }

    public override void CollectObservations() {
        AddVectorObs(a);
    }
    public override void AgentAction(float[] vectorAction, string textAction){
        this.a = vectorAction[0];
        SetReward(-400);
        if (this.a == -1.0f){
            Done();
        }
    }
}
