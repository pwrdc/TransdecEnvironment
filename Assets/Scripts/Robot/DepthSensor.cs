using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class DepthSensor : MonoBehaviour
    {
        private GameObject poolSurface;
        public float offset = 0.0f;

        private Rigidbody rbody;

        void Start()
        {
            rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
        }

        public void Init(GameObject poolSurface)
        {
            this.poolSurface = poolSurface;
        }

        private void FixedUpdate()
        {
            Debug.Log(GetDepth());
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