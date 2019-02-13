using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {
	private Rigidbody rbody;
	private GameObject ship; 


	public float accelerationSpeedLongitudinal = 0.1f;
	public float decelerationSpeedLongitudinal = 0.03f;
	public float maxSpeedLongitudinal = 1f;

    public float accelerationSpeedLateral = 0.1f;
    public float decelerationSpeedLateral = 0.03f;
    public float maxSpeedLateral = 1f;

    public float accelerationSpeedVertical = 0.05f;
	public float decelerationSpeedVertical = 0.02f;
	public float maxSpeedVertical = 1f;

	public float accelerationYaw = 2.5f;
	public float decelerationYaw = 1.0f;
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
		Move();
	}


	/*
		Boat steering (Forward, Upward, RotationX) operate on Transform
	*/
	void Move() {

		movementLongitudinal = InputMovement(movementLongitudinal, "w", "s", accelerationSpeedLongitudinal, decelerationSpeedLongitudinal, maxSpeedLongitudinal);
        movementLateral = InputMovement(movementLateral, "d", "a", accelerationSpeedLateral, decelerationSpeedLateral, maxSpeedLateral);
		movementVertical = InputMovement(movementVertical, "r", "f", accelerationSpeedVertical, decelerationSpeedVertical, maxSpeedVertical);

		ship.transform.Translate(movementLateral*Time.deltaTime, movementVertical*Time.deltaTime, movementLongitudinal*Time.deltaTime);

		movementYaw = InputMovement(movementYaw, "q", "e", accelerationYaw, decelerationYaw, maxYaw);

		ship.transform.Rotate(Vector3.down * movementYaw * Time.deltaTime);
		
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
