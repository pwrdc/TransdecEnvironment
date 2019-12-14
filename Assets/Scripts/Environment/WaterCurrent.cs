// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-02-2019
// ***********************************************************************
// <copyright file="WaterCurrent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneEnvironment
{
    /// <summary>
    /// Class WaterCurrent.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class WaterCurrent : MonoBehaviour
    {
        /// <summary>
        /// The minimum velocity
        /// </summary>
        [SerializeField]
        private float minVelocity = 0.001f;
        /// <summary>
        /// The maximum velocity
        /// </summary>
        [SerializeField]
        private float maxVelocity = 0.005f;
        /// <summary>
        /// The maximum velocity change
        /// </summary>
        [SerializeField]
        private float maxVelocityChange = 0.0005f;
        /// <summary>
        /// The maximum angle change
        /// </summary>
        [SerializeField]
        private float maxAngleChange = 0.08f;
        /// <summary>
        /// The rbody
        /// </summary>
        [HideInInspector]
        private Rigidbody rbody;

        /// <summary>
        /// The angle
        /// </summary>
        private float angle;
        /// <summary>
        /// The radius
        /// </summary>
        private float radius;
        /// <summary>
        /// The current
        /// </summary>
        private Vector3 current;
        /// <summary>
        /// The is enabled
        /// </summary>
        private bool isEnabled = false;


        /// <summary>
        /// Determines whether [is water current].
        /// </summary>
        /// <returns><c>true</c> if [is water current]; otherwise, <c>false</c>.</returns>
        public bool isWaterCurrent() { return isEnabled; }
        /// <summary>
        /// Sets the water current.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void setWaterCurrent(bool isEnabled) { this.isEnabled = isEnabled; }


        /// <summary>
        /// Starts this instance.
        /// </summary>
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


        // Update is called once per frame
        /// <summary>
        /// Updates this instance.
        /// </summary>
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

        /// <summary>
        /// Datas the update.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void DataUpdate(EnvironmentSettings settings)
        {
            isEnabled = settings.isCurrentEnabled;
        }
    }
}