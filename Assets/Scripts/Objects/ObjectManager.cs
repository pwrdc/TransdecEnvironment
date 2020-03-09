using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class ObjectManager : MonoBehaviour
    {
        [Header("Data collection settings")]
        [SerializeField]
        private ObjectSpawnConfiguration objectSpawnConfiguration = null;
        [SerializeField]
        private RandomCameraPosition randomCameraPosition = null;
        [SerializeField]
        private RandomInitNormal randomInitNormal = null;
        [SerializeField]
        private RandomInitOnMesh randomInitOnMesh = null;
        [SerializeField]
        private NoiseSpawner noiseSpawner = null;

        private void Start(){
            RobotAgent.Instance.OnDataCollection.AddListener(OnDataCollection);
        }

        private void OnDataCollection(){
            //Randomize target object position
            if (RobotAgent.Instance.agentSettings.randomizeTargetObjectPositionOnEachStep)
                RandomizeTargetPosition();

            //Randomize camera position
            RandomizeCameraPositionFocusedOnTarget();
        }

        public void RandomizeTargetPosition()
        {
            if (TargetSettings.Instance.cameraType == CameraType.bottomCamera)
            {
                randomInitOnMesh.PutTarget(TargetSettings.Instance.target);
            }
            else if (TargetSettings.Instance.cameraType == CameraType.frontCamera)
            {
                randomInitNormal.PutTarget(TargetSettings.Instance.target);
            }
        }

        public void RandomizeCameraPositionFocusedOnTarget()
        {
            randomCameraPosition.SetNewRobotPos();
            if (noiseSpawner.addNoise)
            {
                noiseSpawner.AddNoise(objectSpawnConfiguration.GetSettingsForActivatedObject(), ObjectConfigurationSettings.Instance.numberOfNoiseToGenerate);
            }
        }
    }
}