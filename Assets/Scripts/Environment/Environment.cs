﻿﻿using UnityEngine;
using System;

namespace Environment
{
    public class Environment : MonoBehaviour
    {
        //Singleton
        private static Environment instance = null;
        public static Environment Instance => Singleton.GetInstance(ref instance);

        public Transform waterSurface;
        
        public bool randomized = false;
        public bool initOnEachStep = false;

        public event Action OnNormalInit;
        public event Action OnRandomizedInit;

        public float GetWaterY()
        {
            if (waterSurface == null)
            {
                return Mathf.Infinity;
            } else
            {
                return waterSurface.transform.position.y;
            }
        }

        public bool IsUnderWater(float y)
        {
            return waterSurface != null && y < waterSurface.position.y;
        }

        void Start()
        {
            Singleton.Initialize(this, ref instance);
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