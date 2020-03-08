﻿using UnityEngine;


namespace Environment
{
    [System.Serializable]
    public class EnvironmentInitValues
    {
        public int minLightAngle = 60;
        public int maxLightAngle = 120;

        [Range(0.0f, 0.3f)]
        public float minIntensivity = 0.1f;
        [Range(0.3f, 1f)]
        public float maxIntensivity = 1f;
        [Range(0.0f, 0.3f)]
        public float minWaterFog = 0.2f;
        [Range(0.2f, 0.6f)]
        public float maxWaterFog = 0.4f;
        public Vector3 minWaterHSV = new Vector3(180, 0, 50);
        public Vector3 maxWaterHSV = new Vector3(250, 100, 100);
        [Header("If true, light is initialized")]
        public bool isLightInitialized = true;
        public int normalLightAngle = 80;
        public float normalLightIntensivity = 0.6f;
        public float normalWaterFog = 0.25f;
        public Color normalWaterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
    }

    public class Environment : MonoBehaviour
    {
        //Singleton
        private static Environment mInstance = null;
        public static Environment Instance => 
            mInstance == null ? (mInstance = FindObjectOfType<Environment>()) : mInstance;

        private bool isEnvironmentRandom = false; //true - water color, water fog and light have random values
        private bool areObjectsRandom = false; //true - objects are created randomly with RandomInit class

        private EnvironmentInitValues environmentInitValues; //copy of RobotAgent environmentValues

#pragma warning disable 0649
        [SerializeField]
        private LightManager lightManager = null;
        [SerializeField]
        private WaterOpacity waterOpacity = null;
        [SerializeField]
        private WaterCurrent waterCurrent = null;
#pragma warning restore 0649

        public Transform waterSurface;
        public Transform poolSurface;
        
        public bool isEnvironmentRandomized = false;
        public bool isEnvironmentInitOnEachStep = false;

        public LightManager Light { get { return lightManager; } }
        public WaterOpacity WaterOpacity { get { return waterOpacity; } }
        public WaterCurrent WaterCurrent { get { return waterCurrent; } }

        void Start()
        {
            RobotAgent.Instance.OnDataEnvironmentValuesUpdate += UpdateData;
            RobotAgent.Instance.OnDataCollection+=OnDataCollection;
        }

        public void Init(EnvironmentInitValues settings)
        {
            environmentInitValues = settings;
        }

        public void Reset(){
            if (isEnvironmentRandomized)
                EnvironmentRandomizedInit();
            else
                EnvironmentNormalInit();
        }

        public void OnDataCollection(){
            //Randomize environment (Water color and light)
            if(isEnvironmentRandomized && isEnvironmentInitOnEachStep){
                EnvironmentRandomizedInit();
            }
        }

        public void EnvironmentNormalInit()
        {
            if(!environmentInitValues.isLightInitialized)
                lightManager.initializeLight(environmentInitValues.normalLightAngle, environmentInitValues.normalLightIntensivity);
            waterOpacity.SetWaterFog(environmentInitValues.normalWaterFog);
            waterOpacity.SetWaterColor(environmentInitValues.normalWaterColor);
        }

        public void EnvironmentRandomizedInit()
        {
            float angle = Utils.GetRandom(environmentInitValues.minLightAngle, environmentInitValues.maxLightAngle);
            float intensitivity = Utils.GetRandom(environmentInitValues.minIntensivity, environmentInitValues.maxIntensivity);
            float percentageIntensitivity = (intensitivity - environmentInitValues.minIntensivity) / (environmentInitValues.maxIntensivity - environmentInitValues.minIntensivity);
            float waterFog = environmentInitValues.minWaterFog + (percentageIntensitivity * (environmentInitValues.maxWaterFog - environmentInitValues.minWaterFog));

            if (!environmentInitValues.isLightInitialized)
                lightManager.initializeLight(angle, intensitivity);

            waterOpacity.SetWaterFog(waterFog);

            float h = Utils.GetRandom(environmentInitValues.minWaterHSV.x, environmentInitValues.maxWaterHSV.x) / 360;
            float s = Utils.GetRandom(environmentInitValues.minWaterHSV.y, environmentInitValues.maxWaterHSV.y) / 100;
            float v = Utils.GetRandom(environmentInitValues.minWaterHSV.z, environmentInitValues.maxWaterHSV.z) / 100;
            Color rgb = Color.HSVToRGB(h, s, v);

            waterOpacity.SetWaterColor(rgb);
        }

        public void UpdateData(EnvironmentInitValues settings)
        {
            environmentInitValues = settings;
        }
    }
}