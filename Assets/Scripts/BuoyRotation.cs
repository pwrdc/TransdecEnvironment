using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyRotation : MonoBehaviour
{

	public float RotationSpeed = 360/50;
	public GameObject Robot;
	private Bounds bounds;


	void Start() {
		bounds = Robot.GetComponent<RobotAgent>().GetComplexBounds(gameObject);
	}


    void Update() {
        transform.RotateAround(bounds.center, new Vector3(0, 1, 0), RotationSpeed);
    }

}
