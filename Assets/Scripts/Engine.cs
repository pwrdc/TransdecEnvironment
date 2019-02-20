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
	public float maxForceLongitudinal = 1f;
	public float maxForceVertical = 1f;
    public float maxForceLateral = 1f;
	public float maxTorqueYaw = 1f;

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
		Move();
	}

	void Move() {
		getMovement("d", "a", "r", "f", "w", "s", "e", "q");
		rbody.AddRelativeForce(maxForceLateral * lateral, maxForceVertical * vertical, maxForceLongitudinal * longitudinal);
		rbody.AddRelativeTorque(0, maxTorqueYaw * yaw, 0);

	}

	public void Move(float X, float Y, float Z, float Yaw) {
		movementLongitudinal = InputMovement(movementLongitudinal, X, accelerationSpeedLongitudinal, decelerationSpeedLongitudinal);
        movementLateral = InputMovement(movementLateral, Y, accelerationSpeedLateral, decelerationSpeedLateral);
		movementVertical = InputMovement(movementVertical, Z, accelerationSpeedVertical, decelerationSpeedVertical);

		ship.transform.Translate(movementLateral * Time.deltaTime * maxSpeedLongitudinal,
								 movementVertical * Time.deltaTime * maxSpeedLateral,
								 movementLongitudinal * Time.deltaTime * maxSpeedVertical);

		movementYaw = InputMovement(movementYaw, Yaw, accelerationYaw, decelerationYaw);

		ship.transform.Rotate(Vector3.down * movementYaw * Time.deltaTime * maxYaw);
	}

	/*
		Computing actuall movement speed
			Speed depends on acceleration and deceleration of an object
			
			Deceleration occurs when no input is given

		Return movement
	*/
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

	float InputMovement(float movement, string keyForward, string keyBack, float accelerationSpeed, float decelerationSpeed, float maxSpeed) {

		if (Input.GetKey(keyForward)) 
			movement += accelerationSpeed;
		else if (Input.GetKey(keyBack))
			movement -= accelerationSpeed;
		else {
			if (movement > decelerationSpeed)
				movement -= decelerationSpeed;
			else if (movement < -decelerationSpeed)
				movement += decelerationSpeed;
			else 
				movement = 0;
		}

		if (movement > 1.0f)
			movement = 1.0f;
		else if (movement < -1.0f) 
			movement = -1.0f;

		return movement;
	}

	float InputMovement(float movement, float target, float accelerationSpeed, float decelerationSpeed) {
		if (target != 0) {
			if (movement < target)
				movement += accelerationSpeed;	
			else if (target < movement)
				movement -= accelerationSpeed;
		}
		else {
			if (movement > decelerationSpeed)
				movement -= decelerationSpeed;
			else if (movement < -decelerationSpeed)
				movement += decelerationSpeed;
			else 
				movement = 0;
		}

		if (Math.Abs(movement) > Math.Abs(target) & Math.Sign(movement) == Math.Sign(target))
			movement = target;
		
		return movement;
	}
}
