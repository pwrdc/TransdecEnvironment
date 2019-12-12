// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="EnvironmentManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneEnvironment
{
    /// <summary>
    /// Initialized values for environment
    /// </summary>
    [System.Serializable]
    public class EnvironmentInitValues
    {
        /// <summary>
        /// The minimum light angle
        /// </summary>
        public int minLightAngle = 60;
        /// <summary>
        /// The maximum light angle
        /// </summary>
        public int maxLightAngle = 120;

        /// <summary>
        /// The minimum intensivity
        /// </summary>
        [Range(0.0f, 0.3f)]
        public float minIntensivity = 0.1f;
        /// <summary>
        /// The maximum intensivity
        /// </summary>
        [Range(0.3f, 1f)]
        public float maxIntensivity = 1f;
        /// <summary>
        /// The minimum water fog
        /// </summary>
        [Range(0.0f, 0.3f)]
        public float minWaterFog = 0.2f;
        /// <summary>
        /// The maximum water fog
        /// </summary>
        [Range(0.2f, 0.6f)]
        public float maxWaterFog = 0.4f;
        /// <summary>
        /// The minimum water HSV
        /// </summary>
        public Vector3 minWaterHSV = new Vector3(180, 0, 50);
        /// <summary>
        /// The maximum water HSV
        /// </summary>
        public Vector3 maxWaterHSV = new Vector3(250, 100, 100);

        /// <summary>
        /// The normal light angle
        /// </summary>
        public int normalLightAngle = 80;
        /// <summary>
        /// The normal light intensivity
        /// </summary>
        public float normalLightIntensivity = 0.6f;
        /// <summary>
        /// The normal water fog
        /// </summary>
        public float normalWaterFog = 0.25f;
        /// <summary>
        /// The normal water color
        /// </summary>
        public Color normalWaterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
    }

    /// <summary>
    /// Settings for environment
    /// </summary>
    [System.Serializable]
    public class EnvironmentSettings
    {
        /// <summary>
        /// The is environment randomized
        /// </summary>
        public bool isEnvironmentRandomized = false;
        /// <summary>
        /// The is environment initialize on each step
        /// </summary>
        public bool isEnvironmentInitOnEachStep = false;
        /// <summary>
        /// The is current enabled
        /// </summary>
        public bool isCurrentEnabled = true;
        /// <summary>
        /// The water surface
        /// </summary>
        public GameObject WaterSurface = null;
    }

    /// <summary>
    /// Manages all environment
    /// </summary>
    public class EnvironmentManager : MonoBehaviour
    {
        /// <summary>
        /// The is environment random
        /// </summary>
        private bool isEnvironmentRandom = false; //true - water color, water fog and light have random values
        /// <summary>
        /// The are objects random
        /// </summary>
        private bool areObjectsRandom = false; //true - objects are created randomly with RandomInit class

        /// <summary>
        /// The environment initialize values
        /// </summary>
        private EnvironmentInitValues environmentInitValues; //copy of RobotAgent environmentValues

#pragma warning disable 0649
        /// <summary>
        /// The light manager
        /// </summary>
        [SerializeField]
        private LightManager lightManager = null;
        /// <summary>
        /// The water opacity
        /// </summary>
        [SerializeField]
        private WaterOpacity waterOpacity = null;
        /// <summary>
        /// The water current
        /// </summary>
        [SerializeField]
        private WaterCurrent waterCurrent = null;
#pragma warning restore 0649


        /// <summary>
        /// Gets the light.
        /// </summary>
        /// <value>The light.</value>
        public LightManager Light { get { return lightManager; } }
        /// <summary>
        /// Gets the water opacity.
        /// </summary>
        /// <value>The water opacity.</value>
        public WaterOpacity WaterOpacity { get { return waterOpacity; } }
        /// <summary>
        /// Gets the water current.
        /// </summary>
        /// <value>The water current.</value>
        public WaterCurrent WaterCurrent { get { return waterCurrent; } }

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            RobotAgent.Instance.OnDataEnvironmentValuesUpdate += UpdateData;   
        }

        /// <summary>
        /// Sets environment to environment init values
        /// </summary>
        public void EnvironmentNormalInit()
        {
            lightManager.initializeLight(environmentInitValues.normalLightAngle, environmentInitValues.normalLightIntensivity);
            waterOpacity.SetWaterFog(environmentInitValues.normalWaterFog);
            waterOpacity.SetWaterColor(environmentInitValues.normalWaterColor);
        }

        /// <summary>
        /// Sets randomized environment
        /// light angle
        /// water color
        /// water fog
        /// </summary>
        public void EnvironmentRandomizedInit()
        {
            float angle = Utils.GetRandom(environmentInitValues.minLightAngle, environmentInitValues.maxLightAngle);
            float intensitivity = Utils.GetRandom(environmentInitValues.minIntensivity, environmentInitValues.maxIntensivity);
            float percentageIntensitivity = (intensitivity - environmentInitValues.minIntensivity) / (environmentInitValues.maxIntensivity - environmentInitValues.minIntensivity);
            float waterFog = environmentInitValues.minWaterFog + (percentageIntensitivity * (environmentInitValues.maxWaterFog - environmentInitValues.minWaterFog));
            lightManager.initializeLight(angle, intensitivity);

            waterOpacity.SetWaterFog(waterFog);

            float h = Utils.GetRandom(environmentInitValues.minWaterHSV.x, environmentInitValues.maxWaterHSV.x) / 360;
            float s = Utils.GetRandom(environmentInitValues.minWaterHSV.y, environmentInitValues.maxWaterHSV.y) / 100;
            float v = Utils.GetRandom(environmentInitValues.minWaterHSV.z, environmentInitValues.maxWaterHSV.z) / 100;
            Color rgb = Color.HSVToRGB(h, s, v);

            waterOpacity.SetWaterColor(rgb);
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(EnvironmentInitValues settings)
        {
            environmentInitValues = settings;
        }
    }
}