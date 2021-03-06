﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class DepthSensor : MonoBehaviour
    {
        public float offset = 0.0f;
        public bool printDepth;

        private Rigidbody rbody;

        void Start()
        {
            rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if(printDepth){
                Debug.Log(GetDepth());
            }
        }

        public float GetDepth()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                return hit.distance;
            }
            return 0;
        }
    }
}