using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Environment;

namespace Robot
{
    public class Engine : MonoBehaviour
    {
        new Rigidbody rigidbody;

        public float drag = 2.0f;
        public float angularDrag = 2.0f;
        public float maxForceLongitudinal = 50;
        public float maxForceVertical = 50;
        public float maxForceLateral = 50;
        public float maxTorqueYaw = 0.5f;

        void Start()
        {
            rigidbody = GetComponentInParent<Rigidbody>();
        }

        void Update()
        {
            LevelSelf();
            if (rigidbody.drag != drag)
                rigidbody.drag = drag;
            if (rigidbody.angularDrag != angularDrag)
                rigidbody.angularDrag = angularDrag;
        }

        public void Move(float Longitudinal, float Lateral, float Vertical, float Yaw)
        {
            if (WaterLevel.IsAbove(transform.position.y))
            {
                rigidbody.AddRelativeForce(maxForceLateral * Lateral, maxForceVertical * Vertical, maxForceLongitudinal * Longitudinal);
                rigidbody.AddRelativeTorque(0, maxTorqueYaw * Yaw, 0);
            }
        }

        void LevelSelf()
        {
            rigidbody.AddRelativeTorque((float)(-Math.Sin(rigidbody.rotation.eulerAngles.x * (Math.PI / 180))),
                                    0,
                                    (float)(-Math.Sin(rigidbody.rotation.eulerAngles.z * (Math.PI / 180))));
        }
    }
}