using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCurrent : MonoBehaviour
{
    //Singleton
    private static WaterCurrent mInstance = null;
    public static WaterCurrent Instance =>
        mInstance == null ? (mInstance = FindObjectOfType<WaterCurrent>()) : mInstance;

    public FloatRange forceRange = new FloatRange(100, 200);
    public float maxVelocityChange = 0.0005f;
    public float maxAngleChange = 0.08f;

    float angle;
    float radius;
    public Vector3 force;

    [ResetParameter("WaterCurrent"), HideInInspector]
    public bool isEnabled = false;

    public Vector3 GetForce()
    {
        if (isEnabled)
            return force;
        else
            return Vector3.zero;
    }

    void Start()
    {
        ResetParameterAttribute.InitializeAll(this);
        angle = Random.Range(0, 360);
        radius = forceRange.GetRandom();
        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);
        force = new Vector3(x, 0, z);
    }

    void Update()
    {
        if (isEnabled)
        {
            // using delta time here ensures that the speed of change will be the same for different frame rates
            float radiusChange = Random.Range(-maxVelocityChange, maxVelocityChange) * Time.deltaTime;
            float angleChange = Random.Range(-maxAngleChange, maxAngleChange) * Time.deltaTime;

            radius += radiusChange;
            if (radius > forceRange.max)
                radius = forceRange.max;
            else if (radius < forceRange.min)
                radius = forceRange.min;

            angle += angleChange;

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            force = new Vector3(x, 0, z);
        }
    }
}