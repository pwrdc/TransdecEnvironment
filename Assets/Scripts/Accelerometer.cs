using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
	private Rigidbody rbody;
	private GameObject robot; 
    private Vector3 lastVelocity;
    private Vector3 acceleration;
    private Vector3 lastAngularVelocity;
    private Vector3 angularAcceleration;
    private Vector3 startRotation;
    // Start is called before the first frame update
    void Start()
    {
		robot = this.transform.parent.gameObject;
		rbody = robot.GetComponent<Rigidbody>();
        lastVelocity = transform.InverseTransformDirection(rbody.velocity);
        lastAngularVelocity = transform.InverseTransformDirection(rbody.angularVelocity);
        startRotation = rbody.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rbody.velocity);
        acceleration = (localVelocity - lastVelocity) / Time.fixedDeltaTime;
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rbody.angularVelocity);
        angularAcceleration = (localAngularVelocity - lastAngularVelocity) / Time.fixedDeltaTime;
        lastVelocity = localVelocity;
        lastAngularVelocity = localAngularVelocity;
    }

    public float[] getAcceleration() {
        /* get value of accelerations: lateral, vertical, longitudinal */
        float[] ret = new float[3];
        ret[0] = acceleration.x;
        ret[1] = acceleration.y;
        ret[2] = acceleration.z;
        return ret;
    }

    public float[] getAngularAcceleration() {
        /* get value of angular accelerations: pitch, yaw, roll */
        float[] ret = new float[3];
        ret[0] = angularAcceleration.x;
        ret[1] = angularAcceleration.y;
        ret[2] = angularAcceleration.z;
        return ret;
    }

    public float[] getRotation() {
        /* get value of angular positions: pitch, yaw, roll */
        float[] ret = new float[3];
        Vector3 rotation = rbody.rotation.eulerAngles;
        ret[0] = normalizeRotation(rotation.x, startRotation.x);
        ret[1] = normalizeRotation(rotation.y, startRotation.y);
        ret[2] = normalizeRotation(rotation.z, startRotation.z);
        return ret;
    }

    private float normalizeRotation(float current, float start){
        float result = (current - start) % 360;
        if (result < 0)
            result += 360;
        result = (result + 180) % 360 - 180;
        return result;
    }
}
