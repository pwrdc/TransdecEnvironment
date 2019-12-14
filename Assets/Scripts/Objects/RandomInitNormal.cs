// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="RandomInitNormal.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Random init of objects on pool
    /// Normal means not on mesh
    /// </summary>
    public class RandomInitNormal : MonoBehaviour, IRandomInit
    {
        /// <summary>
        /// Class ObjectPositionSettings.
        /// </summary>
        [System.Serializable]
        public class ObjectPositionSettings
        {
            /// <summary>
            /// The object
            /// </summary>
            public GameObject obj;
            /// <summary>
            /// The start position
            /// </summary>
            public Vector3 startPosition = new Vector3(0f, 0f, 0f);
            /// <summary>
            /// The x position range
            /// </summary>
            [Range(0f, 10f)]
            public float xPosRange = 0f;
            /// <summary>
            /// The minus x position range
            /// </summary>
            [Range(-10f, 0f)]
            public float minusXPosRange = 0f;
            /// <summary>
            /// The y position range
            /// </summary>
            [Range(0f, 10f)]
            public float yPosRange = 0f;
            /// <summary>
            /// The minus y position range
            /// </summary>
            [Range(-10f, 0f)]
            public float minusYPosRange = 0;
            /// <summary>
            /// The z position range
            /// </summary>
            [Range(0f, 10f)]
            public float zPosRange = 0f;
            /// <summary>
            /// The minus z position range
            /// </summary>
            [Range(-10f, 0f)]
            public float minusZPosRange = -0f;
            /// <summary>
            /// The start rotation
            /// </summary>
            public Vector3 startRotation = new Vector3(0f, 0f, 0f);
            /// <summary>
            /// The x ang range
            /// </summary>
            [Range(0f, 30f)]
            public float xAngRange = 0f;
            /// <summary>
            /// The y ang range
            /// </summary>
            [Range(0f, 30f)]
            public float yAngRange = 0f;
            /// <summary>
            /// The z ang range
            /// </summary>
            [Range(0f, 30f)]
            public float zAngRange = 0f;
            /// <summary>
            /// The allowed rotations
            /// </summary>
            [Range(0f, 360f)]
            public List<float> allowedRotations = new List<float>();

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectPositionSettings"/> class.
            /// </summary>
            /// <param name="obj">The object.</param>
            public ObjectPositionSettings(GameObject obj) { this.obj = obj; }
        }

        /// <summary>
        /// The objects settings
        /// </summary>
        [SerializeField]
        private List<ObjectPositionSettings> objectsSettings;
        /// <summary>
        /// The object configuration settings
        /// </summary>
        private ObjectConfigurationSettings objectConfigurationSettings;

        /// <summary>
        /// The tasks objs
        /// </summary>
        private List<GameObject> tasksObjs = new List<GameObject>();


        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            Utils.GetObjectsInFolder(RobotAgent.Instance.ObjectConfigurationSettings.tasksFolder, out tasksObjs);

            foreach(var obj in tasksObjs)
            {
                var objSettings = GetObjectPositionSettingsForTarget(obj);
                // if object position and rotation are not default save them for randomizer
                objSettings.startPosition = obj.transform.position;
                objSettings.startRotation = obj.transform.eulerAngles;
            }
        }

        //Randomize position of target object
        /// <summary>
        /// Puts the target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void PutTarget(GameObject target)
        {
            int quarter, xCoef, zCoef;
            CalculateCoefficient(out quarter, out xCoef, out zCoef);
            ObjectPositionSettings targetObjectSetting = GetObjectPositionSettingsForTarget(target);

            float xRot = targetObjectSetting.startRotation.x;
            float yRot = targetObjectSetting.startRotation.y;
            float zRot = targetObjectSetting.startRotation.z;
            float xPos = xCoef * targetObjectSetting.startPosition.x;
            float yPos = targetObjectSetting.startPosition.y;
            float zPos = zCoef * targetObjectSetting.startPosition.z;
            // if on the other side, rotate 180
            if (zCoef == -1) yRot += 180f;
            if (objectConfigurationSettings.randomPosition)
            {
                // adjust rotations according to values stored in object (or set in inspector)
                xRot += Utils.GetRandom(-targetObjectSetting.xAngRange, targetObjectSetting.xAngRange);
                yRot += Utils.GetRandom(-targetObjectSetting.yAngRange, targetObjectSetting.yAngRange);
                zRot += Utils.GetRandom(-targetObjectSetting.zAngRange, targetObjectSetting.zAngRange);
                Bounds bounds = Utils.GetComplexBounds(targetObjectSetting.obj);
                // adjust positions according to values stored in the object (or set in inspector); take coefs into consideration
                xPos += Utils.GetRandom(targetObjectSetting.minusXPosRange, targetObjectSetting.xPosRange);
                yPos = Math.Min(yPos + Utils.GetRandom(
                    targetObjectSetting.minusYPosRange,
                    targetObjectSetting.yPosRange), 
                    RobotAgent.Instance.EnvironmentSettings.WaterSurface.transform.position.y - bounds.size.y
                    );
                zPos += Utils.GetRandom(targetObjectSetting.minusZPosRange, targetObjectSetting.zPosRange);
            }
            if (objectConfigurationSettings.randomOrientation)
            {
                // if object has several possible y rotations, pick one randomly
                if (targetObjectSetting.allowedRotations.Count != 0)
                    yRot += -xCoef * zCoef * targetObjectSetting.allowedRotations[Utils.GetRandom(0, targetObjectSetting.allowedRotations.Count)];
            }
            // transform position
            targetObjectSetting.obj.transform.position = new Vector3(xPos, yPos, zPos);
            // transform rotation
            targetObjectSetting.obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
        }

        //Randomize position of all objects
        /// <summary>
        /// Puts all.
        /// </summary>
        public void PutAll()
        {
            foreach (var objSettings in objectsSettings)
            {
                PutTarget(objSettings.obj);
            }
        }

        /// <summary>
        /// Calculates the coefficient.
        /// </summary>
        /// <param name="quarter">The quarter.</param>
        /// <param name="xCoef">The x coef.</param>
        /// <param name="zCoef">The z coef.</param>
        private void CalculateCoefficient(out int quarter, out int xCoef, out int zCoef)
        {               
            // select one of the quarters
            quarter = (int)Utils.GetRandom(0, 4);
            if (objectConfigurationSettings.randomQuarter)
            {
                // divide into binary for particular quarter selection
                xCoef = 2 * (quarter % 2) - 1;
                zCoef = 2 * (quarter / 2 % 2) - 1;
            }
            else
            {
                xCoef = 1;
                zCoef = 1;
            }
        }

        //Get object setting for target, if there is no settings, it creates with defaults Parameters
        /// <summary>
        /// Gets the object position settings for target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>ObjectPositionSettings.</returns>
        private ObjectPositionSettings GetObjectPositionSettingsForTarget(GameObject target)
        {
            foreach(var objectSetting in objectsSettings)
            {
                if (objectSetting.obj == target)
                    return objectSetting;
            }

            objectsSettings.Add(new ObjectPositionSettings(target));
            return objectsSettings[objectsSettings.Count - 1];
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void UpdateData(ObjectConfigurationSettings settings)
        {
            if(objectConfigurationSettings == null)
                objectConfigurationSettings = settings;

            if (objectConfigurationSettings.tasksFolder == settings.tasksFolder || tasksObjs.Count == 0)
                Utils.GetObjectsInFolder(settings.tasksFolder, out tasksObjs);
            objectConfigurationSettings = settings;

        }
    }
}