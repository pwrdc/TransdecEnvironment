// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 10-21-2019
// ***********************************************************************
// <copyright file="ObjectSpawnConfiguration.cs" company="">
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
    /// Settings for spawned object
    /// </summary>
    [System.Serializable]
    public class Settings
    {
        /// <summary>
        /// The target
        /// </summary>
        [HideInInspector]
        public GameObject target;
        /// <summary>
        /// The target annotation
        /// </summary>
        [HideInInspector]
        public GameObject targetAnnotation;
        /// <summary>
        /// The mode
        /// </summary>
        [HideInInspector]
        public CameraType mode;
        /// <summary>
        /// The type
        /// </summary>
        [HideInInspector]
        public ObjectType type;

        /// <summary>
        /// The minimum phi
        /// </summary>
        public float minPhi = -180.0f;
        /// <summary>
        /// The maximum phi
        /// </summary>
        public float maxPhi = 180.0f;
        /// <summary>
        /// The minimum radius
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        /// <summary>
        /// The maximum radius
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        /// <summary>
        /// The camera fov
        /// </summary>
        [Range(30.0f, 90.0f)]
        public float cameraFov = 40.0f;
        /// <summary>
        /// The water level
        /// </summary>
        public float waterLevel = 11.0f;
        /// <summary>
        /// The maximum depth
        /// </summary>
        [Range(0.0f, 11.0f)]
        public float maxDepth = 4.0f;
        /// <summary>
        /// The x ang range
        /// </summary>
        [Range(0.0f, 30.0f)]
        public float xAngRange = 15f;
        /// <summary>
        /// The y ang range
        /// </summary>
        [Range(0.0f, 30.0f)]
        public float yAngRange = 15f;
        /// <summary>
        /// The z ang range
        /// </summary>
        [Range(0.0f, 30.0f)]
        public float zAngRange = 15f;
        //other objects parameters
        /// <summary>
        /// The oth minimum radius
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float othMinRadius = 0.0f;
        /// <summary>
        /// The oth maximum radius
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float othMaxRadius = 4.0f;
        /// <summary>
        /// The oth maximum depth
        /// </summary>
        [Range(0.0f, 11.0f)]
        public float othMaxDepth = 8.0f;
        /// <summary>
        /// The oth x ang range
        /// </summary>
        [Range(0.0f, 90.0f)]
        public float othXAngRange = 90f;
        /// <summary>
        /// The oth y ang range
        /// </summary>
        [Range(0.0f, 90.0f)]
        public float othYAngRange = 90f;
        /// <summary>
        /// The oth z ang range
        /// </summary>
        [Range(0.0f, 90.0f)]
        public float othZAngRange = 90f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public Settings(CameraType mode) { this.mode = mode; }
    }

    /// <summary>
    /// The most important settings for spawning object
    /// </summary>
    public class EssentialSettings
    {
        /// <summary>
        /// The minimum radius
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        /// <summary>
        /// The maximum radius
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        /// <summary>
        /// The water level
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float waterLevel = 3.0f;
        /// <summary>
        /// The maximum depth
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float maxDepth = 10.0f;
        /// <summary>
        /// The camera fov
        /// </summary>
        public float cameraFov = 40.0f;
    }

    /// <summary>
    /// Class for spawning objects
    /// </summary>
    public class ObjectSpawnConfiguration : MonoBehaviour
    {
        /// <summary>
        /// The object settings
        /// </summary>
        [SerializeField]
        private List<Settings> objectSettings;
        /// <summary>
        /// Gets the object settings.
        /// </summary>
        /// <value>The object settings.</value>
        public List<Settings> ObjectSettings { get { return objectSettings; } }
        /// <summary>
        /// The enabled option
        /// </summary>
        [SerializeField]
        private int enabledOption;

        /// <summary>
        /// The essential settings
        /// </summary>
        public Dictionary<ObjectType, EssentialSettings> essentialSettings = new Dictionary<ObjectType, EssentialSettings>()
        {
            { ObjectType.Big, new EssentialSettings() { minRadius = 3, maxRadius = 8, maxDepth = 4, waterLevel = 11 } },
            { ObjectType.Small, new EssentialSettings() { minRadius = 1.5f, maxRadius = 3, maxDepth = 7.5f, waterLevel = 9 } },
            { ObjectType.OnBottom, new EssentialSettings() { minRadius = 1.5f, maxRadius = 4, maxDepth = 9f, waterLevel = 11 } }
        };

        void Awake()
        {
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
        }

        /// <summary>
        /// Called when [validate].
        /// </summary>
        void OnValidate()
        {
            UpdateAllSettingsType();
        }

        /// <summary>
        /// Update all object settings
        /// If user changed values it becomes type of manual
        /// </summary>
        void UpdateAllSettingsType()
        {
            foreach (var setting in objectSettings)
            {
                if (AreSettingsNotEqualToType(setting, essentialSettings[ObjectType.Big]) &&
                    AreSettingsNotEqualToType(setting, essentialSettings[ObjectType.Small]) &&
                    AreSettingsNotEqualToType(setting, essentialSettings[ObjectType.OnBottom]))
                    setting.type = ObjectType.Manual;
            }
        }

        /// <summary>
        /// Checks if object essential settings are not equal to type settings
        /// If user changed some setting, object will become type of manual
        /// </summary>
        /// <param name="objectSetting">The object setting.</param>
        /// <param name="typeToCheck">The type to check.</param>
        /// <returns>true if object settings are changed</returns>
        bool AreSettingsNotEqualToType(Settings objectSetting, EssentialSettings typeToCheck)
        {
            if (objectSetting.minRadius == typeToCheck.minRadius &&
                objectSetting.maxRadius == typeToCheck.maxRadius &&
                objectSetting.waterLevel == typeToCheck.waterLevel &&
                objectSetting.maxDepth == typeToCheck.maxDepth)
                return false;
            return true;
        }

        /// <summary>
        /// Delete object with index
        /// </summary>
        /// <param name="index">The index.</param>
        public void DeleteObject(int index)
        {
            objectSettings.RemoveAt(index);
        }

        /// <summary>
        /// Set camera mode
        /// </summary>
        /// <param name="index">object index</param>
        /// <param name="mode">camera mode (front camera and ...)</param>
        public void SetCameraMode(int index, CameraType mode)
        {
            var objectSetting = objectSettings[index];

            if (objectSetting.type != ObjectType.Manual)
            {
                if (mode == CameraType.bottomCamera && objectSetting.type != ObjectType.OnBottom)
                    SetObjectType(index, ObjectType.OnBottom);
                else if (mode == CameraType.frontCamera && objectSetting.type == ObjectType.OnBottom)
                    SetObjectType(index, ObjectType.Small);
            }

            objectSetting.mode = mode;
        }

        /// <summary>
        /// Sets object type, and its essential values
        /// </summary>
        /// <param name="index">index of object</param>
        /// <param name="type">type of object</param>
        public void SetObjectType(int index, ObjectType type)
        {
            objectSettings[index].type = type;
            objectSettings[index].minRadius = essentialSettings[type].minRadius;
            objectSettings[index].maxRadius = essentialSettings[type].maxRadius;
            objectSettings[index].waterLevel = essentialSettings[type].waterLevel;
            objectSettings[index].maxDepth = essentialSettings[type].maxDepth;
        }

        /// <summary>
        /// Add new object with default values for specific type
        /// </summary>
        /// <param name="mode">camera mode</param>
        /// <param name="type">object type</param>
        /// <param name="target">object</param>
        public void AddNewObject(CameraType mode, ObjectType type, GameObject target)
        {
            objectSettings.Add(new Settings(mode));
            SetObjectType(objectSettings.Count - 1, type);
        }

        /// <summary>
        /// Sets the object target.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="target">The target.</param>
        public void SetObjectTarget(int index, GameObject target)
        {
            objectSettings[objectSettings.Count - 1].target = target;
        }

        /// <summary>
        /// Sets the object target annotation.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="annotation">The annotation.</param>
        public void SetObjectTargetAnnotation(int index, GameObject annotation) { objectSettings[objectSettings.Count - 1].targetAnnotation = annotation; }

        /// <summary>
        /// Inserts the new object.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="type">The type.</param>
        public void InsertNewObject(int index, CameraType mode, ObjectType type)
        {
            objectSettings.Insert(index, new Settings(mode));
            SetObjectType(index, type);
        }


        /// <summary>
        /// Gets the settings for activated object.
        /// </summary>
        /// <returns>Settings.</returns>
        public Settings GetSettingsForActivatedObject() { return objectSettings[enabledOption]; }

        public void UpdateData(TargetSettings settings)
        {
            this.enabledOption = settings.targetIndex;
        }
    }
}