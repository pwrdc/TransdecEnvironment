using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    public class RandomCameraPosition : MonoBehaviour
    {
        private GameObject robot = null;

        private ObjectSpawnConfiguration objectSpawnConfiguration;
        private TargetSettings targetSettings;
        private ObjectConfigurationSettings objectConfigurationSettings;
        private NoiseSpawner noiseSpawner;


        void Start()
        {
            noiseSpawner = GetComponent<NoiseSpawner>();
            robot = RobotAgent.Instance.Robot.transform.gameObject;
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
        }

        public void Init(ObjectSpawnConfiguration objectSpawnConfiguration, ObjectConfigurationSettings objectConfigurationSettings, TargetSettings targetSettings)
        {
            this.objectSpawnConfiguration = objectSpawnConfiguration;
            this.objectConfigurationSettings = objectConfigurationSettings;
            this.targetSettings = targetSettings;
        }

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

        public void UpdateData(TargetSettings settings)
        {
            targetSettings = settings;
        }

        public void UpdateData(ObjectConfigurationSettings settings)
        {
            objectConfigurationSettings = settings;
        }
    }
}