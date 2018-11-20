using UnityEngine;
using MLAgents;

public class RobotAgent : Agent {
    Rigidbody rbody;

	void Start () {
        rbody = GetComponent<Rigidbody>();
	}
}
