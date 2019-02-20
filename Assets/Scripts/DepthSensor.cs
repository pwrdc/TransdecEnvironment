using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSensor : MonoBehaviour
{
    public GameObject waterSurface;
    public float offset = 0.0f;

    [HideInInspector]
    public float depth;

    private Rigidbody rbody;

    // Start is called before the first frame update
    void Start()
    {
		rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float d = waterSurface.transform.position.y - rbody.position.y + offset;
        if (d >= 0)
            depth = d;
        else
            depth = 0f;
    }
}
