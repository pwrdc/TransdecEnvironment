using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class RandomInitNormal : MonoBehaviour, IRandomInit
    {
        [System.Serializable]
        public class ObjectPositionSettings
        {
            public GameObject obj;
            public Vector3 startPosition = new Vector3(0f, 0f, 0f);
            [Range(0f, 10f)]
            public float xPosRange = 0f;
            [Range(-10f, 0f)]
            public float minusXPosRange = 0f;
            [Range(0f, 10f)]
            public float yPosRange = 0f;
            [Range(-10f, 0f)]
            public float minusYPosRange = 0;
            [Range(0f, 10f)]
            public float zPosRange = 0f;
            [Range(-10f, 0f)]
            public float minusZPosRange = -0f;
            public Vector3 startRotation = new Vector3(0f, 0f, 0f);
            [Range(0f, 30f)]
            public float xAngRange = 0f;
            [Range(0f, 30f)]
            public float yAngRange = 0f;
            [Range(0f, 30f)]
            public float zAngRange = 0f;
            [Range(0f, 360f)]
            public List<float> allowedRotations = new List<float>();

            public ObjectPositionSettings(GameObject obj) { this.obj = obj; }
        }

        [SerializeField]
        private List<ObjectPositionSettings> objectsSettings;
        private ObjectConfigurationSettings objectConfigurationSettings;

        private List<GameObject> tasksObjs = new List<GameObject>();



        void Start()
        {
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
            Utils.GetObjectsInFolder(RobotAgent.Instance.ObjectConfigurationSettings.tasksFolder, out tasksObjs);

            foreach (var obj in tasksObjs)
            {
                var objSettings = GetObjectPositionSettingsForTarget(obj);
                objSettings.startPosition = obj.transform.position;
                objSettings.startRotation = obj.transform.eulerAngles;
            }
        }

        public void Init(ObjectConfigurationSettings settings)
        {
            objectConfigurationSettings = settings;
        }

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
            if (zCoef == -1) yRot += 180f;
            if (objectConfigurationSettings.randomPosition)
            {
                xRot += Utils.GetRandom(-targetObjectSetting.xAngRange, targetObjectSetting.xAngRange);
                yRot += Utils.GetRandom(-targetObjectSetting.yAngRange, targetObjectSetting.yAngRange);
                zRot += Utils.GetRandom(-targetObjectSetting.zAngRange, targetObjectSetting.zAngRange);
                Bounds bounds = Utils.GetComplexBounds(targetObjectSetting.obj);
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
                if (targetObjectSetting.allowedRotations.Count != 0)
                    yRot += -xCoef * zCoef * targetObjectSetting.allowedRotations[Utils.GetRandom(0, targetObjectSetting.allowedRotations.Count)];
            }
            targetObjectSetting.obj.transform.position = new Vector3(xPos, yPos, zPos);
            targetObjectSetting.obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
        }

        public void PutAll()
        {
            foreach (var objSettings in objectsSettings)
            {
                PutTarget(objSettings.obj);
            }
        }

        private void CalculateCoefficient(out int quarter, out int xCoef, out int zCoef)
        {
            quarter = (int)Utils.GetRandom(0, 4);
            if (objectConfigurationSettings.randomQuarter)
            {
                xCoef = 2 * (quarter % 2) - 1;
                zCoef = 2 * (quarter / 2 % 2) - 1;
            }
            else
            {
                xCoef = 1;
                zCoef = 1;
            }
        }

        private ObjectPositionSettings GetObjectPositionSettingsForTarget(GameObject target)
        {
            foreach (var objectSetting in objectsSettings)
            {
                if (objectSetting.obj == target)
                    return objectSetting;
            }

            objectsSettings.Add(new ObjectPositionSettings(target));
            return objectsSettings[objectsSettings.Count - 1];
        }

        public void UpdateData(ObjectConfigurationSettings settings)
        {
            if (objectConfigurationSettings == null)
                objectConfigurationSettings = settings;

            if (objectConfigurationSettings.tasksFolder == settings.tasksFolder || tasksObjs.Count == 0)
                Utils.GetObjectsInFolder(settings.tasksFolder, out tasksObjs);
            objectConfigurationSettings = settings;

        }
    }
}