using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Engine : MonoBehaviour {
	private Rigidbody rbody;
	private GameObject ship; 

	public bool keyboard = false;

	public float accelerationSpeedLongitudinal = 0.1f;
	public float decelerationSpeedLongitudinal = 0.03f;
	public float maxSpeedLongitudinal = 1f;

    public float accelerationSpeedLateral = 0.1f;
    public float decelerationSpeedLateral = 0.03f;
    public float maxSpeedLateral = 1f;

    public float accelerationSpeedVertical = 0.05f;
	public float decelerationSpeedVertical = 0.02f;
	public float maxSpeedVertical = 1f;

	public float accelerationYaw = 0.05f;
	public float decelerationYaw = 0.02f;
	public float maxYaw = 30.0f;

	private float movementLongitudinal = 0;
    private float movementLateral = 0;
	private float movementVertical = 0;
	private float movementYaw = 0;

	// Use this for initialization
	void Start () {
		ship = this.transform.parent.gameObject;
		rbody = ship.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (keyboard)
			Move();
	}


	/*
		Boat steering (Forward, Upward, RotationX) operate on Transform
	*/
	public void Move() {

		movementLongitudinal = InputMovement(movementLongitudinal, "w", "s", accelerationSpeedLongitudinal, decelerationSpeedLongitudinal);
        movementLateral = InputMovement(movementLateral, "d", "a", accelerationSpeedLateral, decelerationSpeedLateral);
		movementVertical = InputMovement(movementVertical, "r", "f", accelerationSpeedVertical, decelerationSpeedVertical);

		ship.transform.Translate(movementLateral * Time.deltaTime * maxSpeedLongitudinal,
								 movementVertical * Time.deltaTime * maxSpeedLateral,
								 movementLongitudinal * Time.deltaTime * maxSpeedVertical);

		movementYaw = InputMovement(movementYaw, "q", "e", accelerationYaw, decelerationYaw);

		ship.transform.Rotate(Vector3.down * movementYaw * Time.deltaTime * maxYaw);
		
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
	float InputMovement(float movement, string keyForward, string keyBack, float accelerationSpeed, float decelerationSpeed) {

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
