// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="Accelerometer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    /// <summary>
    /// Calculates all accelerometer data of object
    /// </summary>
    public class Accelerometer : MonoBehaviour
    {
        /// <summary>
        /// The rbody
        /// </summary>
        private Rigidbody rbody;
        /// <summary>
        /// The last velocity
        /// </summary>
        private Vector3 lastVelocity;
        /// <summary>
        /// The acceleration
        /// </summary>
        private Vector3 acceleration;
        /// <summary>
        /// The last angular velocity
        /// </summary>
        private Vector3 lastAngularVelocity;
        /// <summary>
        /// The angular acceleration
        /// </summary>
        private Vector3 angularAcceleration;
        /// <summary>
        /// The start rotation
        /// </summary>
        private Vector3 startRotation;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            rbody = this.transform.parent.gameObject.GetComponent<Rigidbody>();
            lastVelocity = transform.InverseTransformDirection(rbody.velocity);
            lastAngularVelocity = transform.InverseTransformDirection(rbody.angularVelocity);
            startRotation = rbody.rotation.eulerAngles;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rbody.velocity);
            acceleration = (localVelocity - lastVelocity) / Time.fixedDeltaTime;
            Vector3 localAngularVelocity = transform.InverseTransformDirection(rbody.angularVelocity);
            angularAcceleration = (localAngularVelocity - lastAngularVelocity) / Time.fixedDeltaTime;
            lastVelocity = localVelocity;
            lastAngularVelocity = localAngularVelocity;
        }

        /// <summary>
        /// Get (lateral, vertical, longitudinal) accelaration values
        /// </summary>
        /// <returns>acceleration values</returns>
        public float[] GetAcceleration()
        {
            float[] ret = new float[3];
            ret[0] = acceleration.x;
            ret[1] = acceleration.y;
            ret[2] = acceleration.z;
            return ret;
        }

        /// <summary>
        /// Get (pitch, yaw, roll) accelaration values
        /// </summary>
        /// <returns>angular acceleration</returns>
        public float[] GetAngularAcceleration()
        {
            float[] ret = new float[3];
            ret[0] = angularAcceleration.x;
            ret[1] = angularAcceleration.y;
            ret[2] = angularAcceleration.z;
            return ret;
        }

        /// <summary>
        /// Get normalized (0, 360) rotation (pitch, yaw, roll) of object
        /// </summary>
        /// <returns>rotation of object</returns>
        public float[] GetRotation()
        {
            float[] ret = new float[3];
            Vector3 rotation = rbody.rotation.eulerAngles;
            ret[0] = NormalizeRotation(rotation.x, startRotation.x);
            ret[1] = NormalizeRotation(rotation.y, startRotation.y);
            ret[2] = NormalizeRotation(rotation.z, startRotation.z);
            return ret;
        }

        /// <summary>
        /// Normalizes the rotation.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="start">The start.</param>
        /// <returns>System.Single.</returns>
        private float NormalizeRotation(float current, float start)
        {
            float result = (current - start) % 360;
            if (result < 0)
                result += 360;
            result = (result + 180) % 360 - 180;
            return result;
        }
    }
}