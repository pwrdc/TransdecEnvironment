using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class WaterCurrent : MonoBehaviour
    {
        [SerializeField]
        private float minVelocity = 0.001f;
        [SerializeField]
        private float maxVelocity = 0.005f;
        [SerializeField]
        private float maxVelocityChange = 0.0005f;
        [SerializeField]
        private float maxAngleChange = 0.08f;
        [HideInInspector]
        private Rigidbody rbody;

        private float angle;
        private float radius;
        private Vector3 current;
        [ResetParameter("WaterCurrent")] private bool isEnabled = false;

        public bool isWaterCurrent() { return isEnabled; }
        public void setWaterCurrent(bool isEnabled) { this.isEnabled = isEnabled; }

        void Start()
        {
            ResetParameterAttribute.InitializeAll(this);
            angle = Random.Range(0, 360);
            radius = Random.Range(minVelocity, maxVelocity);
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            current = new Vector3(x, 0, z);
            rbody = GameObject.Find("Robot").GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!isEnabled)
                return;

            float radiusChange = Random.Range(-maxVelocityChange, maxVelocityChange);
            float angleChange = Random.Range(-maxAngleChange, maxAngleChange);

            radius += radiusChange;
            if (radius > maxVelocity)
                radius = maxVelocity;
            else if (radius < minVelocity)
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
}