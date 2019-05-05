using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public float speed = 100f;
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * speed);
    }


    void Update()
    {

    }
}
