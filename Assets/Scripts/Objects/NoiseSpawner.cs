// ***********************************************************************
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
    /// <summary>
    /// Class NoiseSpawner.
    /// Implements the <see cref="UnityEngine.MonoBehaviour" />
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class NoiseSpawner : MonoBehaviour
    {
        /// <summary>
        /// The robot
        /// </summary>
        private GameObject robot;

        /// <summary>
        /// The other objs
        /// </summary>
        private List<GameObject> otherObjs = new List<GameObject>();
        /// <summary>
        /// The other objs mesh
        /// </summary>
        private List<MeshRenderer[]> otherObjsMesh = new List<MeshRenderer[]>();

        /// <summary>
        /// The target settings
        /// </summary>
        private TargetSettings targetSettings;
        /// <summary>
        /// The object configuration settings
        /// </summary>
        private ObjectConfigurationSettings objectConfigurationSettings;

        /// <summary>
        /// The radius of generated object
        /// </summary>
        private float radiusOfGeneratedObject;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            robot = RobotAgent.Instance.Robot.gameObject;
            RobotAgent.Instance.OnDataTargetUpdate += UpdateData;
            RobotAgent.Instance.OnDataConfigurationUpdate += UpdateData;
        }

        /// <summary>
        /// Determines whether [is overriding object] [the specified object].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if [is overriding object] [the specified object]; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Gets the other objs.
        /// </summary>
        /// <returns>List&lt;GameObject&gt;.</returns>
        public List<GameObject> GetOtherObjs() { return otherObjs; }

        //Setting up new position for objects in order to Settings
        /// <summary>
        /// Sets the oth new position.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="setting">The setting.</param>
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
        /// <summary>
        /// Adds the noise.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="numberOfNoiseToGenerate">The number of noise to generate.</param>
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

        /// <summary>
        /// Sets the radius of generated object.
        /// </summary>
        /// <param name="radiusOfGeneratedObject">The radius of generated object.</param>
        public void SetRadiusOfGeneratedObject(float radiusOfGeneratedObject) { this.radiusOfGeneratedObject = radiusOfGeneratedObject; }

        /// <summary>
        /// Gets the random objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>List&lt;GameObject&gt;.</returns>
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

        /// <summary>
        /// Gets the raycast hit.
        /// </summary>
        /// <returns>GameObject.</returns>
        GameObject GetRaycastHit() //Raycast from robot position to target
        {
            int layerMask = (1 << 9) | (1 << 11);
            float dist = Vector3.Distance(targetSettings.targetAnnotation.transform.position, robot.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(robot.transform.position, targetSettings.targetAnnotation.transform.position - robot.transform.position, out hit, dist, layerMask))
                return hit.transform.gameObject;
            return null;
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
            if(objectConfigurationSettings == null)
                objectConfigurationSettings = settings;

            if (objectConfigurationSettings.noiseFolder == settings.noiseFolder || otherObjs.Count == 0)
                Utils.GetObjectsAndMeshInFolder(settings.noiseFolder, out otherObjs, out otherObjsMesh);

            objectConfigurationSettings = settings;
        }
    }
}