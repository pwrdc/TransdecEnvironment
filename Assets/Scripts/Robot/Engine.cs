using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Robot
{
    public class Engine : MonoBehaviour
    {
        private Rigidbody rbody;
        private GameObject robot;

        public float drag = 2.0f;
        public float angularDrag = 2.0f;
        public float maxForceLongitudinal = 50;
        public float maxForceVertical = 50;
        public float maxForceLateral = 50;
        public float maxTorqueYaw = 0.5f;

        private float longitudinal = 0;
        private float lateral = 0;
        private float yaw = 0;
        private float vertical = 0;

        private GameObject waterSurface;
        float top;

        void Start()
        {
            Physics.gravity = new Vector3(0, -5.0f, 0);
            rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
        }

        void Update()
        {
            selfLevel();
            if (rbody.position.y >= top)
                rbody.useGravity = true;
            else
                rbody.useGravity = false;
            if (rbody.drag != drag)
                rbody.drag = drag;
            if (rbody.angularDrag != angularDrag)
                rbody.angularDrag = angularDrag;
        }

        public void Init(GameObject waterSurface)
        {
            this.waterSurface = waterSurface;
            top = waterSurface.transform.position.y;
        }

        public int isAboveSurface()
        {
            if (rbody.position.y >= top)
                return 1;
            else
                return 0;
        }

        public void Move(float Longitudinal, float Lateral, float Vertical, float Yaw)
        {
            lateral = Lateral;
            longitudinal = Longitudinal;
            vertical = Vertical;
            yaw = Yaw;
            if (!rbody.useGravity)
            {
                rbody.AddRelativeForce(maxForceLateral * lateral, maxForceVertical * vertical, maxForceLongitudinal * longitudinal);
                rbody.AddRelativeTorque(0, maxTorqueYaw * yaw, 0);
            }
        }

        void selfLevel()
        {
            rbody.AddRelativeTorque((float)(-Math.Sin(rbody.rotation.eulerAngles.x * (Math.PI / 180))),
                                    0,
                                    (float)(-Math.Sin(rbody.rotation.eulerAngles.z * (Math.PI / 180))));
        }
    }
}