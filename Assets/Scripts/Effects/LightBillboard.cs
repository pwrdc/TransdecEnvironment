using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Basically we want this thing to rotate 
 * around it's local Y axis towards currently rendering camera.
 * This could be a shader, but it'd require implementing 
 * LookAt in shader language.
 */
public class LightBillboard : MonoBehaviour
{
    Vector3 up;

    void Start()
    {
        up = transform.up;
    }

    void OnRenderObject()
    {
        Vector3 observer = Camera.current.transform.position;
        Vector3 onPlane = new Vector3(observer.x, transform.position.y, observer.z);
        transform.LookAt(onPlane, up);
    }
}
