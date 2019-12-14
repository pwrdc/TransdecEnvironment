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
    [System.Serializable]
    public class Settings
    {
        [HideInInspector]
        public GameObject target;
        [HideInInspector]
        public GameObject targetAnnotation;
        [HideInInspector]
        public CameraType mode;
        [HideInInspector]
        public ObjectType type;

        public float minPhi = -180.0f;
        public float maxPhi = 180.0f;
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        [Range(30.0f, 90.0f)]
        public float cameraFov = 40.0f;
        public float waterLevel = 11.0f;
        [Range(0.0f, 11.0f)]
        public float maxDepth = 4.0f;
        [Range(0.0f, 30.0f)]
        public float xAngRange = 15f;
        [Range(0.0f, 30.0f)]
        public float yAngRange = 15f;
        [Range(0.0f, 30.0f)]
        public float zAngRange = 15f;
        //other objects parameters
        [Range(0.0f, 10.0f)]
        public float othMinRadius = 0.0f;
        [Range(0.0f, 10.0f)]
        public float othMaxRadius = 4.0f;
        [Range(0.0f, 11.0f)]
        public float othMaxDepth = 8.0f;
        [Range(0.0f, 90.0f)]
        public float othXAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othYAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othZAngRange = 90f;

        public Settings(CameraType mode) { this.mode = mode; }
    }

    [System.Serializable]
    public class EssentialSettings
    {
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        [Range(0.0f, 10.0f)]
        public float waterLevel = 3.0f;
        [Range(0.0f, 10.0f)]
        public float maxDepth = 10.0f;
        public float cameraFov = 40.0f;
    }

    public class ObjectSpawnConfiguration : MonoBehaviour
    {
        [SerializeField]
        private EssentialSettings objectBigSettings = new EssentialSettings()
        {
            minRadius = 3,
            maxRadius = 8,
            maxDepth = 4,
            waterLevel = 11
        };
        [SerializeField]
        private EssentialSettings objectSmallSettings = new EssentialSettings()
        {
            minRadius = 1.5f,
            maxRadius = 3,
            maxDepth = 7.5f,
            waterLevel = 9
        };
        [SerializeField]
        private EssentialSettings objectBottomSettings = new EssentialSettings()
        {
            minRadius = 1.5f,
            maxRadius = 4,
            maxDepth = 9f,
            waterLevel = 11
        };
        [SerializeField]
        private List<Settings> objectSettings;
        public List<Settings> ObjectSettings { get { return objectSettings; } }
        [SerializeField]
        private int enabledOption;

        public Dictionary<ObjectType, EssentialSettings> essentialSettings;

        void Start()
        {
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
        }

        public void Init(TargetSettings settings)
        {
            this.enabledOption = settings.targetIndex;
            essentialSettings = new Dictionary<ObjectType, EssentialSettings>()
            {
                { ObjectType.Big, objectBigSettings },
                { ObjectType.Small, objectSmallSettings },
                { ObjectType.OnBottom, objectBottomSettings }
            };
        }

        void OnValidate()
        {
            UpdateAllSettingsType();
        }

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

        bool AreSettingsNotEqualToType(Settings objectSetting, EssentialSettings typeToCheck)
        {
            if (objectSetting.minRadius == typeToCheck.minRadius &&
                objectSetting.maxRadius == typeToCheck.maxRadius &&
                objectSetting.waterLevel == typeToCheck.waterLevel &&
                objectSetting.maxDepth == typeToCheck.maxDepth)
                return false;
            return true;
        }

        public void DeleteObject(int index)
        {
            objectSettings.RemoveAt(index);
        }

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

        public void SetObjectType(int index, ObjectType type)
        {
            objectSettings[index].type = type;
            objectSettings[index].minRadius = essentialSettings[type].minRadius;
            objectSettings[index].maxRadius = essentialSettings[type].maxRadius;
            objectSettings[index].waterLevel = essentialSettings[type].waterLevel;
            objectSettings[index].maxDepth = essentialSettings[type].maxDepth;
        }

        public void AddNewObject(CameraType mode, ObjectType type, GameObject target)
        {
            objectSettings.Add(new Settings(mode));
            SetObjectType(objectSettings.Count - 1, type);
        }

        public void SetObjectTarget(int index, GameObject target)
        {
            objectSettings[objectSettings.Count - 1].target = target;
        }

        public void SetObjectTargetAnnotation(int index, GameObject annotation) { objectSettings[objectSettings.Count - 1].targetAnnotation = annotation; }

        public void InsertNewObject(int index, CameraType mode, ObjectType type)
        {
            objectSettings.Insert(index, new Settings(mode));
            SetObjectType(index, type);
        }


        public Settings GetSettingsForActivatedObject() { return objectSettings[enabledOption]; }

        public void UpdateData(TargetSettings settings)
        {
            this.enabledOption = settings.targetIndex;
        }
    }
}