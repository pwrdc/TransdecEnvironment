using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    [System.Serializable]
    public class ObjectConfigurationSettings
    {
        public bool addNoise = false;
        public bool setFocusedObjectInCenter = false;
        public bool setObjectAlwaysVisible = true; public bool addCompetitionObjectsAsNoise = true;
        [HideInInspector]
        public GameObject noiseFolder = null;
        [HideInInspector]
        public GameObject tasksFolder = null;
        public bool randomQuarter = true;
        public bool randomPosition = true;
        public bool randomOrientation = true;
        public int numberOfNoiseToGenerate = 5;
    }

    public class ObjectManager : MonoBehaviour
    {
        [Header("Data collection settings")]
        [SerializeField]
        private ObjectSpawnConfiguration objectSpawnConfiguration = new ObjectSpawnConfiguration();
        [SerializeField]
        private RandomCameraPosition randomCameraPosition = null;
        [SerializeField]
        private RandomInitNormal randomInitNormal = null;
        [SerializeField]
        private RandomInitOnMesh randomInitOnMesh = null;
        [SerializeField]
        private NoiseSpawner noiseSpawner = null;

        private ObjectConfigurationSettings objectConfigurationSettings;
        private TargetSettings targetSettings;

        void Start()
        {
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
        }

        public void RandomizeObjectsPositionsOnInit()
        {
            randomInitNormal.PutAll();
            randomInitOnMesh.PutAll();
        }

        public void Init(ObjectConfigurationSettings objectConfigurationSettings, TargetSettings targetSettings)
        {
            this.targetSettings = targetSettings;
            this.objectConfigurationSettings = objectConfigurationSettings;
            noiseSpawner.Init(objectConfigurationSettings, targetSettings);
            objectSpawnConfiguration.Init(targetSettings);
            randomCameraPosition.Init(objectSpawnConfiguration, objectConfigurationSettings, targetSettings);
            randomInitNormal.Init(objectConfigurationSettings);
            randomInitOnMesh.Init(objectConfigurationSettings);
        }

        public void RandomizeTargetPosition()
        {
            if (targetSettings.cameraType == CameraType.bottomCamera)
            {
                randomInitOnMesh.PutTarget(targetSettings.target);
            }
            else if (targetSettings.cameraType == CameraType.frontCamera)
            {
                randomInitNormal.PutTarget(targetSettings.target);
            }
        }

        public void RandomizeCameraPositionFocusedOnTarget()
        {
            randomCameraPosition.SetNewRobotPos();
            if (objectConfigurationSettings.addNoise)
            {
                noiseSpawner.AddNoise(objectSpawnConfiguration.GetSettingsForActivatedObject(), objectConfigurationSettings.numberOfNoiseToGenerate);
            }
        }

        public ObjectSpawnConfiguration GetSpawnConfiguration() { return objectSpawnConfiguration; }
        public RandomCameraPosition GetRandomPosition() { return randomCameraPosition; }
        public RandomInitNormal GetRandomInitNormal() { return randomInitNormal; }
        public RandomInitOnMesh GetRandomInitOnMesh() { return randomInitOnMesh; }
        public NoiseSpawner GetNoiseSpawner() { return noiseSpawner; }

        public void UpdateData(ObjectConfigurationSettings settings)
        {
            objectConfigurationSettings = settings;
        }

        public void UpdateData(TargetSettings settings)
        {
            targetSettings = settings;
        }
    }
}