﻿using UnityEngine;
using System;

namespace Environment
{
    public class Environment : Singleton<Environment>
    {
        
        public bool randomized = false;
        public bool initOnEachStep = false;

        public event Action OnNormalInit;
        public event Action OnRandomizedInit;

        protected override void Start()
        {
            base.Start();
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