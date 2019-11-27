// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-23-2019
// ***********************************************************************
// <copyright file="Engine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Robot
{
    /// <summary>
    /// Class Engine.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Engine : MonoBehaviour
    {
        /// <summary>
        /// The rbody
        /// </summary>
        private Rigidbody rbody;
        /// <summary>
        /// The robot
        /// </summary>
        private GameObject robot;

        /// <summary>
        /// The drag
        /// </summary>
        public float drag = 2.0f;
        /// <summary>
        /// The angular drag
        /// </summary>
        public float angularDrag = 2.0f;
        /// <summary>
        /// The maximum force longitudinal
        /// </summary>
        public float maxForceLongitudinal = 50;
        /// <summary>
        /// The maximum force vertical
        /// </summary>
        public float maxForceVertical = 50;
        /// <summary>
        /// The maximum force lateral
        /// </summary>
        public float maxForceLateral = 50;
        /// <summary>
        /// The maximum torque yaw
        /// </summary>
        public float maxTorqueYaw = 0.5f;

        /// <summary>
        /// The longitudinal
        /// </summary>
        private float longitudinal = 0;
        /// <summary>
        /// The lateral
        /// </summary>
        private float lateral = 0;
        /// <summary>
        /// The yaw
        /// </summary>
        private float yaw = 0;
        /// <summary>
        /// The vertical
        /// </summary>
        private float vertical = 0;

        /// <summary>
        /// The water surface
        /// </summary>
        private GameObject waterSurface;
        /// <summary>
        /// The top
        /// </summary>
        float top;

        // Use this for initialization
        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            Physics.gravity = new Vector3(0, -5.0f, 0);
            rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
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

        /// <summary>
        /// Initializes the specified water surface.
        /// </summary>
        /// <param name="waterSurface">The water surface.</param>
        public void Init(GameObject waterSurface)
        {
            this.waterSurface = waterSurface;
            top = waterSurface.transform.position.y;
        }

        /// <summary>
        /// Determines whether [is above surface].
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int isAboveSurface()
        {
            if (rbody.position.y >= top)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Moves the specified longitudinal.
        /// </summary>
        /// <param name="Longitudinal">The longitudinal.</param>
        /// <param name="Lateral">The lateral.</param>
        /// <param name="Vertical">The vertical.</param>
        /// <param name="Yaw">The yaw.</param>
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

        /// <summary>
        /// Selfs the level.
        /// </summary>
        void selfLevel()
        {
            rbody.AddRelativeTorque((float)(-Math.Sin(rbody.rotation.eulerAngles.x * (Math.PI / 180))),
                                    0,
                                    (float)(-Math.Sin(rbody.rotation.eulerAngles.z * (Math.PI / 180))));
        }
    }
}