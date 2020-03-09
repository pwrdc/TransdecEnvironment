﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects{
    public class ObjectConfigurationSettings : MonoBehaviour
    {
        private static ObjectConfigurationSettings mInstance;
        public static ObjectConfigurationSettings Instance => 
            mInstance == null ? (mInstance = FindObjectOfType<ObjectConfigurationSettings>()) : mInstance;
        public bool setObjectAlwaysVisible = true; 
        public bool addCompetitionObjectsAsNoise = true;
        
        public GameObject noiseFolder = null;
        private GameObject previousNoiseFolder=null;
        [HideInInspector]
        public List<GameObject> noiseGameObjects;

        public GameObject tasksFolder = null;
        private GameObject previousTasksFolder=null;
        [HideInInspector]
        public List<GameObject> tasksGameObjects;

        public bool randomQuarter = true;
        public bool randomPosition = true;
        public bool randomOrientation = true;
        public int numberOfNoiseToGenerate = 5;

        [HideInInspector] [ResetParameter] public bool addNoise;
        [HideInInspector] [ResetParameter] public bool setFocusedObjectInCenter;

        private void Start(){
            noiseFolder.SetActive(false);
            RobotAcademy.Instance.onResetParametersChanged+=ApplyResetParameters;
            RobotAgent.Instance.OnReset+=OnReset;
        }

        private void Update(){
            if (noiseFolder != previousNoiseFolder) {
                Utils.GetObjectsInFolder(noiseFolder, out noiseGameObjects);
                previousNoiseFolder=noiseFolder;
            }
            if (tasksFolder != previousTasksFolder) {
                Utils.GetObjectsInFolder(tasksFolder, out tasksGameObjects);
                previousTasksFolder=tasksFolder;
            }
            noiseFolder.SetActive(addNoise);
        }

        void OnApplicationQuit()
        {

            foreach (var obj in tasksGameObjects)
            {
                obj.SetActive(true);
            }

            Objects.ObjectConfigurationSettings.Instance.noiseFolder.SetActive(false);
        }

        void OnReset(){
            if(RobotAgent.Instance.agentSettings.dataCollection){
                foreach (var obj in tasksGameObjects)
                {
                    if (obj != TargetSettings.Instance.target){
                        obj.SetActive(false);
                    }
                }
            }
        }

        private void ApplyResetParameters(){
            addNoise = RobotAcademy.Instance.IsResetParameterTrue("EnableNoise");
            setFocusedObjectInCenter = RobotAcademy.Instance.IsResetParameterTrue("SetFocusedObjectInCenter");
        }
    }
}