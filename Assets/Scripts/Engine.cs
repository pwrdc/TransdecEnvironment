using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {
	private Rigidbody rbody;
	private GameObject ship; 


	public float accelerationSpeedForward = 0.1f;
	public float decelerationSpeedForward = 0.03f;
	public float maxSpeedForward = 1f;

	public float accelerationSpeedUpward = 0.05f;
	public float decelerationSpeedUpward = 0.02f;
	public float maxSpeedUpward = 1f;

	public float accelerationRotation = 2.5f;
	public float decelerationRotation = 1.0f;
	public float maxRotation = 30.0f;

	private float movementForward = 0;
	private float movementUpward = 0;
	private float movementRotationX = 0;

	// Use this for initialization
	void Start () {
		ship = this.transform.parent.gameObject;
		rbody = ship.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}


	/*
		Boat steering (Forward, Upward, RotationX) operate on Transform
	*/
	void Move() {

		movementForward = InputMovement(movementForward, "w", "s", accelerationSpeedForward, decelerationSpeedForward, maxSpeedForward);
		movementUpward = InputMovement(movementUpward, "a", "d", accelerationSpeedUpward, decelerationSpeedUpward, maxSpeedUpward);

		ship.transform.Translate(0f, movementUpward*Time.deltaTime, movementForward*Time.deltaTime);

		movementRotationX = InputMovement(movementRotationX, "e", "q", accelerationRotation, decelerationRotation, maxRotation);

		ship.transform.Rotate(Vector3.down * movementRotationX * Time.deltaTime);
		
	}

	/*
		Computing actuall movement speed
			Speed depends on acceleration and deceleration of an object
			
			Deceleration occurs when no input is given

		Return movement
	*/
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

		if (movement > maxSpeed)
			movement = maxSpeed;
		else if (movement < -maxSpeed) 
			movement = -maxSpeed;

		return movement;
	}
}
