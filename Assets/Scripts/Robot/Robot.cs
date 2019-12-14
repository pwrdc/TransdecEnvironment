// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-23-2019
// ***********************************************************************
// <copyright file="Robot.cs" company="">
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
    /// Class Robot.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Robot : MonoBehaviour
    {
        /// <summary>
        /// The engine
        /// </summary>
        private Engine _engine;
        /// <summary>
        /// The depth sensor
        /// </summary>
        private DepthSensor _depthSensor;
        /// <summary>
        /// The accelerometer
        /// </summary>
        private Accelerometer _accelerometer;
        /// <summary>
        /// The water surface
        /// </summary>
        private GameObject waterSurface;

        /// <summary>
        /// Gets the engine.
        /// </summary>
        /// <value>The engine.</value>
        public Engine Engine { get { return _engine; } }
        /// <summary>
        /// Gets the depth sensor.
        /// </summary>
        /// <value>The depth sensor.</value>
        public DepthSensor DepthSensor { get { return _depthSensor; } }
        /// <summary>
        /// Gets the accelerometer.
        /// </summary>
        /// <value>The accelerometer.</value>
        public Accelerometer Accelerometer { get { return _accelerometer; } }

        void Start()
        {
            _engine = GetComponentInChildren<Engine>();
            _depthSensor = GetComponentInChildren<DepthSensor>();
            _accelerometer = GetComponentInChildren<Accelerometer>();

            waterSurface = RobotAgent.Instance.EnvironmentSettings.WaterSurface;

            _engine.Init(waterSurface);
            _depthSensor.Init(waterSurface);
        }
    }
}