﻿using UnityEngine;
using System;

namespace Environment
{
    public class Environment : MonoBehaviour
    {
        //Singleton
        private static Environment mInstance = null;
        public static Environment Instance => 
            mInstance == null ? (mInstance = FindObjectOfType<Environment>()) : mInstance;

        public Transform waterSurface;
        
        public bool randomized = false;
        public bool initOnEachStep = false;

        public event Action OnNormalInit;
        public event Action OnRandomizedInit;

        public bool isUnderwater(float y)
        {
            return waterSurface != null && y < waterSurface.position.y;
        }

        void Start()
        {
            RobotAgent.Instance.OnDataCollection.AddListener(OnDataCollection);
            RobotAgent.Instance.OnReset.AddListener(OnReset);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && randomized)
                EnvironmentRandomizedInit();
        }

        public void OnReset(){
            if (randomized)
                EnvironmentRandomizedInit();
            else
                EnvironmentNormalInit();
        }

        public void OnDataCollection(){
            //Randomize environment (Water color and light)
            if(randomized && initOnEachStep){
                EnvironmentRandomizedInit();
            }
        }

        public void EnvironmentNormalInit()
        {
            OnNormalInit?.Invoke();
        }

        public void EnvironmentRandomizedInit()
        {
            OnRandomizedInit?.Invoke();
        }
    }
}