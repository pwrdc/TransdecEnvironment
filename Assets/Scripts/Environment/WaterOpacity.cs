// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-02-2019
// ***********************************************************************
// <copyright file="WaterOpacity.cs" company="">
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
    /// Class WaterOpacity.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class WaterOpacity : MonoBehaviour
    {
        /// <summary>
        /// The water color
        /// </summary>
        private Color waterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
        /// <summary>
        /// The water fog
        /// </summary>
        private float waterFog = 0.25f;
        /// <summary>
        /// Gets the water fog.
        /// </summary>
        /// <value>The water fog.</value>
        public float WaterFog { get { return waterFog; } }

        /// <summary>
        /// The underwater
        /// </summary>
        private bool underwater;


        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            RenderSettings.fogMode = FogMode.Exponential;
            RobotAgent.Instance.OnDataEnvironmentValuesUpdate += UpdateData;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update()
        {
            if (waterColor != RenderSettings.fogColor)
                RenderSettings.fogColor = waterColor;
            if (waterFog != RenderSettings.fogDensity)
                RenderSettings.fogDensity = waterFog;
            if ((transform.position.y < RobotAgent.Instance.EnvironmentSettings.WaterSurface.transform.position.y) != underwater)
            {
                underwater = (transform.position.y < RobotAgent.Instance.EnvironmentSettings.WaterSurface.transform.position.y);
                if (underwater) SetUnderwater();
                else SetNormal();
            }
        }

        /// <summary>
        /// Sets the color of the water.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetWaterColor(Color color) { waterColor = color; }
        /// <summary>
        /// Sets the water fog.
        /// </summary>
        /// <param name="fog">The fog.</param>
        public void SetWaterFog(float fog) { waterFog = fog; }

        /// <summary>
        /// Sets the normal.
        /// </summary>
        private void SetNormal()
        {
            RenderSettings.fog = false;
        }

        /// <summary>
        /// Sets the underwater.
        /// </summary>
        private void SetUnderwater()
        {
            RenderSettings.fog = true;
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(EnvironmentInitValues settings)
        {
            waterColor = settings.normalWaterColor;
            waterFog = settings.normalWaterFog;
        }
    }
}