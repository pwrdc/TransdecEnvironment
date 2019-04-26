using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCurrent : MonoBehaviour
{
	public float minVelocity = 0.001f;
	public float maxVelocity = 0.005f; 
	public float maxVelocityChange = 0.0005f; 
	public float maxAngleChange = 0.08f;
	[HideInInspector]
    public Rigidbody rbody;

    float angle;
	float radius;
	Vector3 current;


    System.Random rnd;

    void Start()
    {
        rnd = new System.Random();
        angle = getRandom(0, 360);
        radius = getRandom(minVelocity, maxVelocity);
        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);
        current = new Vector3(x, 0, z);
    }

    float getRandom(float min, float max)
    {
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    // Update is called once per frame
    public void AddCurrentToBoat()
    {
        float radiusChange = getRandom(-maxVelocityChange, maxVelocityChange);
        float angleChange = getRandom(-maxAngleChange, maxAngleChange);

        radius += radiusChange;
        if(radius > maxVelocity)
        	radius = maxVelocity;
        else if(radius < minVelocity)
        	radius = minVelocity;

        angle += angleChange;

        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);

        current = new Vector3(x, 0, z);

        Vector3 robotVelocity = rbody.velocity;
        Vector3 newVelocity = robotVelocity + current;
        rbody.velocity = newVelocity;  
    }
}
