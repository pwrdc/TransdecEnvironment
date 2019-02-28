﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Engine : MonoBehaviour {
	private Rigidbody rbody;
	private GameObject robot; 

	public bool keyboard = false;

	public float drag = 5.0f;
	public float angularDrag = 5.0f;
	public float maxForceLongitudinal = 100f;
	public float maxForceVertical = 100f;
    public float maxForceLateral = 100f;
	public float maxTorqueYaw = 1f;

	private float longitudinal = 0;
    private float lateral = 0;
	private float yaw = 0;
	private float vertical = 0;

	public GameObject waterSurface;
	float top;

	// Use this for initialization
	void Start () {
		Physics.gravity = new Vector3(0, -2.0f, 0);
		robot = this.transform.parent.gameObject;
		rbody = robot.GetComponent<Rigidbody>();
		top = waterSurface.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		selfLevel();
		if (rbody.drag != drag)
			rbody.drag = drag;
		if (rbody.angularDrag != angularDrag)
			rbody.angularDrag = angularDrag;
		if (keyboard)
			Move();
	}

	public int isAboveSurface() {
		if (rbody.position.y >= top)
			return 1;
		else
			return 0;
	}

	void Move() {
		getMovement("d", "a", "r", "f", "w", "s", "e", "q");
		if (rbody.position.y >= top)
			rbody.useGravity = true;
		else {
			rbody.useGravity = false;
			rbody.AddRelativeForce(maxForceLateral * lateral, maxForceVertical * vertical, maxForceLongitudinal * longitudinal);
			rbody.AddRelativeTorque(0, maxTorqueYaw * yaw, 0);
		}
	}

	public void Move(float Longitudinal, float Lateral, float Vertical, float Yaw) {
		lateral = Lateral;
		longitudinal = Longitudinal;
		vertical = Vertical;
		yaw = Yaw;
		if (rbody.position.y >= top)
			rbody.useGravity = true;
		else {
			rbody.useGravity = false;
			rbody.AddRelativeForce(maxForceLateral * lateral, maxForceVertical * vertical, maxForceLongitudinal * longitudinal);
			rbody.AddRelativeTorque(0, maxTorqueYaw * yaw, 0);
		}
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

	void selfLevel() {
		rbody.AddRelativeTorque((float)(-Math.Sin(rbody.rotation.eulerAngles.x * (Math.PI / 180))),
								0,
								(float)(-Math.Sin(rbody.rotation.eulerAngles.z * (Math.PI / 180))));
	}
}
