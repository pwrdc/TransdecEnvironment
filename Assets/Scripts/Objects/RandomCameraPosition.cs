// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="RandomCameraPosition.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    /// <summary>
    /// Generates random position of camera, when its called
    /// </summary>
    public class RandomCameraPosition : MonoBehaviour
    {
        /// <summary>
        /// The robot
        /// </summary>
        private GameObject robot = null;

        /// <summary>
        /// The object spawn configuration
        /// </summary>
        private ObjectSpawnConfiguration objectSpawnConfiguration;
        /// <summary>
        /// The target settings
        /// </summary>
        private TargetSettings targetSettings;
        /// <summary>
        /// The object configuration settings
        /// </summary>
        private ObjectConfigurationSettings objectConfigurationSettings;
        /// <summary>
        /// The noise spawner
        /// </summary>
        private NoiseSpawner noiseSpawner;


        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            noiseSpawner = GetComponent<NoiseSpawner>();
            robot = RobotAgent.Instance.Robot.transform.gameObject;
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
        }

        /// <summary>
        /// Initializes the specified object spawn configuration.
        /// </summary>
        /// <param name="objectSpawnConfiguration">The object spawn configuration.</param>
        public void Init(ObjectSpawnConfiguration objectSpawnConfiguration)
        {
            this.objectSpawnConfiguration = objectSpawnConfiguration;
        }

        /// <summary>
        /// Setup new agent position that looks on object
        /// </summary>
        public void SetNewRobotPos()
        {
            Settings setting = objectSpawnConfiguration.GetSettingsForActivatedObject();
            Vector3 newPos = Utils.GetComplexBounds(targetSettings.target).center;
            float xRot = Utils.GetRandom(-setting.xAngRange, setting.xAngRange);
            float yRot = Utils.GetRandom(-setting.yAngRange, setting.yAngRange);
            float zRot = Utils.GetRandom(-setting.zAngRange, setting.zAngRange);
            float r;

            newPos.y = Utils.GetRandom(setting.maxDepth, setting.waterLevel);

            if (setting.mode.Equals(CameraType.bottomCamera))
            {
                if (objectConfigurationSettings.setFocusedObjectInCenter)
                    r = 0;
                else
                    r = Utils.GetRandom(0, ((newPos.y - Utils.GetComplexBounds(targetSettings.target).center.y) * (setting.cameraFov / 100)));
            }
            else
            {
                r = Utils.GetRandom(setting.minRadius, setting.maxRadius);
            }

            float theta = Utils.GetRandom(setting.minPhi, setting.maxPhi) - 90;
            theta = theta * 3.14f / 180;

            newPos.x += r * Mathf.Cos(theta);
            newPos.z += r * Mathf.Sin(theta);

            noiseSpawner.SetRadiusOfGeneratedObject(r);
            robot.transform.position = newPos;
            if (setting.mode.Equals(CameraType.bottomCamera))
            {
                robot.transform.eulerAngles = new Vector3(0, Utils.GetRandom(0, 360), 0);
            }
            else
            {
                robot.transform.LookAt(Utils.GetComplexBounds(targetSettings.targetAnnotation).center);
                if (!objectConfigurationSettings.setFocusedObjectInCenter)
                    robot.transform.eulerAngles = new Vector3(robot.transform.eulerAngles.x + xRot, robot.transform.eulerAngles.y + yRot, robot.transform.eulerAngles.z + zRot);
            }
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(TargetSettings settings)
        {
            targetSettings = settings;
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(ObjectConfigurationSettings settings)
        {
            objectConfigurationSettings = settings;
        }
    }
}