using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class RandomInitNormal : MonoBehaviour
    {   
        private ObjectPositionSettings[] objectsSettings;
        public GameObject tasksFolder;

        void Start()
        {
            objectsSettings=tasksFolder.GetComponentsInChildren<ObjectPositionSettings>();
            RobotAgent.Instance.OnReset.AddListener(OnReset);
        }

        void OnReset(){
            if(RobotAgent.Instance.agentSettings.dataCollection){
                foreach (var obj in objectsSettings)
                {
                    if (obj.gameObject != TargetSettings.Instance.target){
                        obj.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void PutTarget(GameObject target){
            foreach (var objSettings in objectsSettings){
                if(objSettings.gameObject==target){
                    PutTarget(objSettings);
                }
            }
        }

        public void PutTarget(ObjectPositionSettings targetObjectSetting)
        {
            int quarter, xCoef, zCoef;
            CalculateCoefficient(out quarter, out xCoef, out zCoef);
            GameObject target=targetObjectSetting.gameObject;

            float xRot = targetObjectSetting.startRotation.x;
            float yRot = targetObjectSetting.startRotation.y;
            float zRot = targetObjectSetting.startRotation.z;
            float xPos = xCoef * targetObjectSetting.startPosition.x;
            float yPos = targetObjectSetting.startPosition.y;
            float zPos = zCoef * targetObjectSetting.startPosition.z;
            if (zCoef == -1) yRot += 180f;
            if (ObjectConfigurationSettings.Instance.randomPosition)
            {
                xRot += Utils.GetRandom(-targetObjectSetting.xAngRange, targetObjectSetting.xAngRange);
                yRot += Utils.GetRandom(-targetObjectSetting.yAngRange, targetObjectSetting.yAngRange);
                zRot += Utils.GetRandom(-targetObjectSetting.zAngRange, targetObjectSetting.zAngRange);
                Bounds bounds = Utils.GetComplexBounds(targetObjectSetting.obj);
                xPos += Utils.GetRandom(targetObjectSetting.minusXPosRange, targetObjectSetting.xPosRange);
                yPos = Math.Min(yPos + Utils.GetRandom(
                    targetObjectSetting.minusYPosRange,
                    targetObjectSetting.yPosRange),
                    Environment.Environment.Instance.waterSurface.position.y - bounds.size.y
                    );
                zPos += Utils.GetRandom(targetObjectSetting.minusZPosRange, targetObjectSetting.zPosRange);
            }
            if (ObjectConfigurationSettings.Instance.randomOrientation)
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
                PutTarget(objSettings);
            }
        }

        private void CalculateCoefficient(out int quarter, out int xCoef, out int zCoef)
        {
            quarter = (int)Utils.GetRandom(0, 4);
            if (ObjectConfigurationSettings.Instance.randomQuarter)
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
            return null;
        }
    }
}