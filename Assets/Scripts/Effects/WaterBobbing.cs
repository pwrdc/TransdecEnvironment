using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBobbing : MonoBehaviour
{
    public float amplitude=0.02f;
    public float speed=0.8f;
    float startY;
    float timeOffset;

    // 2*PI is sin functions interval
    const float interval = 2 * Mathf.PI;

    // the object needs to be enabled after changing its y position for it to take effect
    // else it will just oscilate around its old y
    void OnEnable()
    {
        startY = transform.position.y;
        // without tihs all instances would move in sync
       
        timeOffset = Random.value * interval;
    }

    void Update()
    {
        // sin looks better than perlin noise from my observation
        float yPosition = startY+Mathf.Sin(Time.time * speed + timeOffset) * amplitude;
        transform.position =new Vector3(transform.position.x, yPosition, transform.position.z);    
    }
}
