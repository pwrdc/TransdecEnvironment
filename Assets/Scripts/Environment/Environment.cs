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
        public Transform poolSurface;
        
        public bool isEnvironmentRandomized = false;
        public bool isEnvironmentInitOnEachStep = false;

        public event Action OnNormalInit;
        public event Action OnRandomizedInit;


        void Start()
        {
            RobotAgent.Instance.OnDataCollection+=OnDataCollection;
            RobotAgent.Instance.OnReset+=OnReset;
        }

        public void OnReset(){
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
            OnNormalInit.Invoke();
        }

        public void EnvironmentRandomizedInit()
        {
            OnRandomizedInit.Invoke();
        }
    }
}