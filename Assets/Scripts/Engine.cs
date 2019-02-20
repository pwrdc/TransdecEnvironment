using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Engine : MonoBehaviour {
	private Rigidbody rbody;
	private GameObject robot; 

	public bool keyboard = false;

	public float drag = 2.0f;
	public float angularDrag = 2.0f;
	public float maxForceLongitudinal = 50f;
	public float maxForceVertical = 50f;
    public float maxForceLateral = 50f;
	public float maxTorqueYaw = 0.5f;

	private float longitudinal = 0;
    private float lateral = 0;
	private float yaw = 0;
	private float vertical = 0;

	// Use this for initialization
	void Start () {
		robot = this.transform.parent.gameObject;
		rbody = robot.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (rbody.drag != drag)
			rbody.drag = drag;
		if (rbody.angularDrag != angularDrag)
			rbody.angularDrag = angularDrag;
		if (keyboard)
			Move();
	}

	void Move() {
		getMovement("d", "a", "r", "f", "w", "s", "e", "q");
		rbody.AddRelativeForce(maxForceLateral * lateral, maxForceVertical * vertical, maxForceLongitudinal * longitudinal);
		rbody.AddRelativeTorque(0, maxTorqueYaw * yaw, 0);
	}

	public void Move(float Longitudinal, float Lateral, float Vertical, float Yaw) {
		lateral = Lateral;
		longitudinal = Longitudinal;
		vertical = Vertical;
		yaw = Yaw;
		rbody.AddRelativeForce(maxForceLateral * lateral, maxForceVertical * vertical, maxForceLongitudinal * longitudinal);
		rbody.AddRelativeTorque(0, maxTorqueYaw * yaw, 0);
	}

	float[] getMovement(string right, string left, string upward, string downward, string forward, string backward, string turnRight, string turnLeft) {
		float[] ret = new float[4];
		if (Input.GetKey(right) == Input.GetKey(left))
			lateral = 0.0f;
		else if (Input.GetKey(right))
			lateral = 1.0f;
		else if (Input.GetKey(left))
			lateral = -1.0f;
		if (Input.GetKey(upward) == Input.GetKey(downward))
			vertical = 0.0f;
		else if (Input.GetKey(upward))
			vertical = 1.0f;
		else if (Input.GetKey(downward))
			vertical = -1.0f;
		if (Input.GetKey(forward) == Input.GetKey(backward))
			longitudinal = 0.0f;
		else if (Input.GetKey(forward))
			longitudinal = 1.0f;
		else if (Input.GetKey(backward))
			longitudinal = -1.0f;
		if (Input.GetKey(turnRight) == Input.GetKey(turnLeft))
			yaw = 0.0f;
		else if (Input.GetKey(turnRight))
			yaw = 1.0f;
		else if (Input.GetKey(turnLeft))
			yaw = -1.0f;
		return ret;
	}
}
