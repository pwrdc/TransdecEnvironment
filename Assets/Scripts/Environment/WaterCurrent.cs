using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneEnvironment
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
        private bool isEnabled = false;


        public bool isWaterCurrent() { return isEnabled; }
        public void setWaterCurrent(bool isEnabled) { this.isEnabled = isEnabled; }


        void Start()
        {
            angle = Utils.GetRandom(0, 360);
            radius = Utils.GetRandom(minVelocity, maxVelocity);
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            current = new Vector3(x, 0, z);
            rbody = GameObject.Find("Robot").GetComponent<Rigidbody>();
            RobotAgent.Instance.OnDataEnvironmentUpdate += DataUpdate;
        }


        void Update()
        {
            if (!isEnabled)
                return;

            float radiusChange = Utils.GetRandom(-maxVelocityChange, maxVelocityChange);
            float angleChange = Utils.GetRandom(-maxAngleChange, maxAngleChange);

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

        public void DataUpdate(EnvironmentSettings settings)
        {
            isEnabled = settings.isCurrentEnabled;
        }
    }
}