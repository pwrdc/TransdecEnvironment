// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="ObjectManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Stores all information for object configuration
    /// </summary>
    [System.Serializable]
    public class ObjectConfigurationSettings
    {
        /// <summary>
        /// The add noise
        /// </summary>
        public bool addNoise = false;
        /// <summary>
        /// The set focused object in center
        /// </summary>
        public bool setFocusedObjectInCenter = false;
        /// <summary>
        /// The set object always visible
        /// </summary>
        public bool setObjectAlwaysVisible = true;  //when adding noise, make sure that noise won't override object
        /// <summary>
        /// The add competition objects as noise
        /// </summary>
        public bool addCompetitionObjectsAsNoise = true;
        /// <summary>
        /// The noise folder
        /// </summary>
        public GameObject noiseFolder = null;
        /// <summary>
        /// The tasks folder
        /// </summary>
        public GameObject tasksFolder = null;
        /// <summary>
        /// The random quarter
        /// </summary>
        public bool randomQuarter = true;
        /// <summary>
        /// The random position
        /// </summary>
        public bool randomPosition = true;
        /// <summary>
        /// The random orientation
        /// </summary>
        public bool randomOrientation = true;
        /// <summary>
        /// The number of noise to generate
        /// </summary>
        public int numberOfNoiseToGenerate = 5;
    }

    /// <summary>
    /// Manages all objects and its position
    /// </summary>
    public class ObjectManager : MonoBehaviour
    {
        /// <summary>
        /// The object spawn configuration
        /// </summary>
        [Header("Data collection settings")]
        [SerializeField]
        private ObjectSpawnConfiguration objectSpawnConfiguration = new ObjectSpawnConfiguration();
        /// <summary>
        /// The random camera position
        /// </summary>
        [SerializeField]
        private RandomCameraPosition randomCameraPosition = null;
        /// <summary>
        /// The random initialize normal
        /// </summary>
        [SerializeField]
        private RandomInitNormal randomInitNormal = null;
        /// <summary>
        /// The random initialize on mesh
        /// </summary>
        [SerializeField]
        private RandomInitOnMesh randomInitOnMesh = null;
        /// <summary>
        /// The noise spawner
        /// </summary>
        [SerializeField]
        private NoiseSpawner noiseSpawner = null;

        /// <summary>
        /// The object configuration settings
        /// </summary>
        private ObjectConfigurationSettings objectConfigurationSettings;
        /// <summary>
        /// The target settings
        /// </summary>
        private TargetSettings targetSettings;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            randomCameraPosition.Init(objectSpawnConfiguration);
        }

        /// <summary>
        /// Randomize all objects position
        /// Use on start
        /// </summary>
        public void RandomizeObjectsPositionsOnInit()
        {
            randomInitNormal.PutAll();
            randomInitOnMesh.PutAll();
        }

        /// <summary>
        /// Randomize Target object position
        /// Use for Collecting Data
        /// </summary>
        public void RandomizeTargetPosition()
        {
            if(targetSettings.cameraType == CameraType.bottomCamera)
            {
                randomInitOnMesh.PutTarget(targetSettings.target);
            }
            else if(targetSettings.cameraType == CameraType.frontCamera)
            {
                randomInitNormal.PutTarget(targetSettings.target);
            }
        }

        /// <summary>
        /// Randomize Camera position which is focused on target object
        /// Use for Collecting Data
        /// </summary>
        public void RandomizeCameraPositionFocusedOnTarget()
        {
            randomCameraPosition.SetNewRobotPos();
            if (objectConfigurationSettings.addNoise)
            {
                noiseSpawner.AddNoise(objectSpawnConfiguration.GetSettingsForActivatedObject(), objectConfigurationSettings.numberOfNoiseToGenerate);
            }
        }

        /// <summary>
        /// Gets the spawn configuration.
        /// </summary>
        /// <returns>ObjectSpawnConfiguration.</returns>
        public ObjectSpawnConfiguration GetSpawnConfiguration() { return objectSpawnConfiguration; }
        /// <summary>
        /// Gets the random position.
        /// </summary>
        /// <returns>RandomCameraPosition.</returns>
        public RandomCameraPosition GetRandomPosition() { return randomCameraPosition; }
        /// <summary>
        /// Gets the random initialize normal.
        /// </summary>
        /// <returns>RandomInitNormal.</returns>
        public RandomInitNormal GetRandomInitNormal() { return randomInitNormal; }
        /// <summary>
        /// Gets the random initialize on mesh.
        /// </summary>
        /// <returns>RandomInitOnMesh.</returns>
        public RandomInitOnMesh GetRandomInitOnMesh() { return randomInitOnMesh; }
        /// <summary>
        /// Gets the noise spawner.
        /// </summary>
        /// <returns>NoiseSpawner.</returns>
        public NoiseSpawner GetNoiseSpawner() { return noiseSpawner; }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(ObjectConfigurationSettings settings)
        {
            objectConfigurationSettings = settings;
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(TargetSettings settings)
        {
            targetSettings = settings;
        }
    }
}