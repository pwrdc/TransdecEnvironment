using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSensor : MonoBehaviour
{
    public GameObject waterSurface;
    public float offset = 0.0f;

    private Rigidbody rbody;

    void Start()
    {
		rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
    }

    public float GetDepth() {
        float depth;
        float d = waterSurface.transform.position.y - rbody.position.y + offset;
        if (d >= 0)
            depth = d;
        else
            depth = 0f;
        return depth;
    }
}
