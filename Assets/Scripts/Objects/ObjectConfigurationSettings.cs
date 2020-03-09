using System.Collections;
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

        public bool randomQuarter = true;
        public bool randomPosition = true;
        public bool randomOrientation = true;
        public int numberOfNoiseToGenerate = 5;
        [HideInInspector] [ResetParameter] public bool setFocusedObjectInCenter;

        private void Start(){
            RobotAcademy.Instance.onResetParametersChanged.AddListener(ApplyResetParameters);
        }
        private void ApplyResetParameters(){
            setFocusedObjectInCenter = RobotAcademy.Instance.IsResetParameterTrue("SetFocusedObjectInCenter");
        }
    }
}