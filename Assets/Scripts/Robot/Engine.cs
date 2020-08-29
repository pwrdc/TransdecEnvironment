using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Environment;

namespace Robot
{
    public class Engine : MonoBehaviour
    {
        private Rigidbody rbody;

        public float drag = 2.0f;
        public float angularDrag = 2.0f;
        public float maxForceLongitudinal = 50;
        public float maxForceVertical = 50;
        public float maxForceLateral = 50;
        public float maxTorqueYaw = 0.5f;

        bool isUnderwater = true;

        void Start()
        {
            rbody = transform.parent.GetComponent<Rigidbody>();
        }

        void Update()
        {
            isUnderwater = Environment.Environment.Instance.IsUnderWater(transform.position.y);
            LevelSelf();
            if (rbody.drag != drag)
                rbody.drag = drag;
            if (rbody.angularDrag != angularDrag)
                rbody.angularDrag = angularDrag;
        }

        public void Move(float Longitudinal, float Lateral, float Vertical, float Yaw)
        {
            if (isUnderwater)
            {
                rbody.AddRelativeForce(maxForceLateral * Lateral, maxForceVertical * Vertical, maxForceLongitudinal * Longitudinal);
                rbody.AddRelativeTorque(0, maxTorqueYaw * Yaw, 0);
            }
        }

        void LevelSelf()
        {
            rbody.AddRelativeTorque((float)(-Math.Sin(rbody.rotation.eulerAngles.x * (Math.PI / 180))),
                                    0,
                                    (float)(-Math.Sin(rbody.rotation.eulerAngles.z * (Math.PI / 180))));
        }
    }
}