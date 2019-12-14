﻿// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Szymo
// Created          : 09-20-2019
//
// Last Modified By : Szymo
// Last Modified On : 09-22-2019
// ***********************************************************************
// <copyright file="NoiseSpawner.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class NoiseSpawner : MonoBehaviour
    {
        private GameObject robot;

        private List<GameObject> otherObjs = new List<GameObject>();
        private List<MeshRenderer[]> otherObjsMesh = new List<MeshRenderer[]>();

        private TargetSettings targetSettings;
        private ObjectConfigurationSettings objectConfigurationSettings;

        private float radiusOfGeneratedObject;

        void Start()
        {
            robot = RobotAgent.Instance.Robot.gameObject;
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
        }

        public void Init(ObjectConfigurationSettings objectConfigurationSettings, TargetSettings targetSettings)
        {
            this.objectConfigurationSettings = objectConfigurationSettings;
            this.targetSettings = targetSettings;
        }

        bool IsOverridingObject(GameObject obj)
        {
            //Return true if object is overriding target, otherwise return false
            Bounds objBounds = Utils.GetComplexBounds(obj);
            Bounds targetBounds = Utils.GetComplexBounds(targetSettings.targetAnnotation);

            Vector3[] boxCoordOfTarget = Utils.GetBoxCoord(targetBounds);
            Vector3[] boxCoordOfObject = Utils.GetBoxCoord(objBounds);

            Vector3 positionOfObject = obj.transform.position;
            //first equation is XY second is XZ
            Vector2[] lineEquationMin = Utils.calculateEquationOf3DLine(boxCoordOfTarget[0], robot.transform.position);
            Vector2[] lineEquationMax = Utils.calculateEquationOf3DLine(boxCoordOfTarget[1], robot.transform.position);

            //if object is 
            if (Utils.isPointInObject(boxCoordOfObject[0], lineEquationMin, lineEquationMax) ||
                Utils.isPointInObject(boxCoordOfObject[1], lineEquationMin, lineEquationMax) ||
                Utils.isPointInObject(positionOfObject, lineEquationMin, lineEquationMax))
            {
                return true;
            }
            return false;
        }

        public List<GameObject> GetOtherObjs() { return otherObjs; }

        //Setting up new position for objects in order to Settings
        public void SetOthNewPos(GameObject obj, Settings setting)
        {
            Vector3 newPos = Utils.GetComplexBounds(targetSettings.targetAnnotation).center;

            float xRot = Utils.GetRandom(-setting.othXAngRange, setting.othXAngRange);
            float yRot = Utils.GetRandom(-setting.othYAngRange, setting.othYAngRange);
            float zRot = Utils.GetRandom(-setting.othZAngRange, setting.othZAngRange);
            float r = Utils.GetRandom(setting.othMinRadius, setting.othMaxRadius);
            float theta = Utils.GetRandom(0, 2 * Mathf.PI);
            newPos.x += r * Mathf.Cos(theta);
            newPos.y = Utils.GetRandom(setting.othMaxDepth, setting.waterLevel - 0.2f);
            newPos.z += r * Mathf.Sin(theta);
            obj.transform.position = newPos;
            obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);

            if (objectConfigurationSettings.setObjectAlwaysVisible)
            {
                if (IsOverridingObject(obj)) //Check if object is overriding target
                {
                    r = Utils.GetRandom(radiusOfGeneratedObject, setting.othMaxRadius);
                    newPos.x += r * Mathf.Cos(theta);
                    newPos.y = Utils.GetRandom(radiusOfGeneratedObject, setting.waterLevel - 0.2f);
                    newPos.z += r * Mathf.Sin(theta);
                }
            }

            //Set new position and rotation
            obj.transform.position = newPos;
            obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
        }

        //Add new noise to scene, 
        public void AddNoise(Settings settings, int numberOfNoiseToGenerate)
        {
            List<GameObject> objToChose = GetRandomObjects(GetOtherObjs(), numberOfNoiseToGenerate);
            foreach (GameObject obj in objToChose) SetOthNewPos(obj, settings);
            foreach (GameObject obj in otherObjs) obj.SetActive(false);
            foreach (GameObject obj in objToChose) obj.SetActive(true);

            if (objectConfigurationSettings.setObjectAlwaysVisible) //Erase all objects that cover target
            {
                GameObject obj = GetRaycastHit();
                while (obj != null && obj != targetSettings.target && obj != targetSettings.targetAnnotation)
                {
                    obj.SetActive(false);
                    obj = GetRaycastHit();
                }
            }
        }

        public void SetRadiusOfGeneratedObject(float radiusOfGeneratedObject) { this.radiusOfGeneratedObject = radiusOfGeneratedObject; }

        List<GameObject> GetRandomObjects(List<GameObject> objects, int amount)
        {
            List<GameObject> objToChose = new List<GameObject>();
            foreach (GameObject obj in objects) objToChose.Add(obj);
            while (objToChose.Count > amount)
            {
                objToChose.RemoveAt(Utils.GetRandom(0, objToChose.Count));
            }
            return objToChose;
        }

        GameObject GetRaycastHit() //Raycast from robot position to target
        {
            int layerMask = (1 << 9) | (1 << 11);
            float dist = Vector3.Distance(targetSettings.targetAnnotation.transform.position, robot.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(robot.transform.position, targetSettings.targetAnnotation.transform.position - robot.transform.position, out hit, dist, layerMask))
                return hit.transform.gameObject;
            return null;
        }

        public void UpdateData(TargetSettings settings)
        {
            targetSettings = settings;
        }

        public void UpdateData(ObjectConfigurationSettings settings)
        {
            if(objectConfigurationSettings == null)
                objectConfigurationSettings = settings;

            if (objectConfigurationSettings.noiseFolder == settings.noiseFolder || otherObjs.Count == 0)
                Utils.GetObjectsAndMeshInFolder(settings.noiseFolder, out otherObjs, out otherObjsMesh);

            objectConfigurationSettings = settings;
        }
    }
}