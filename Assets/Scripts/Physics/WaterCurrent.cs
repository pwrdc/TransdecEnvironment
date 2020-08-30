using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{

    public class WaterCurrent : Randomized
    {
        static WaterCurrent instance = null;
        public static WaterCurrent Instance => Singleton.GetInstance(ref instance);

        public FloatParameter forceRange = new FloatParameter(50, 20, 100);
        public FloatParameter velocityChange = new FloatParameter(2, 1, 5);
        public FloatParameter angleChange = new FloatParameter(5, 1, 10);

        float angle;
        float radius;
        public Vector3 force;

        [ResetParameter("WaterCurrent"), HideInInspector]
        public bool isEnabled = false;

        public Vector3 GetForce()
        {
            if (isEnabled)
                return force;
            else
                return Vector3.zero;
        }

        public override void Start()
        {
            base.Start();
            Singleton.Initialize(this, ref instance);
            ResetParameterAttribute.InitializeAll(this);
            parameters.Add(forceRange);
            parameters.Add(velocityChange);
            parameters.Add(angleChange);
            angle = Random.Range(0, 360);
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            force = new Vector3(x, 0, z);
        }

        public override void ApplyParameters()
        {
            base.ApplyParameters();
            radius = forceRange.value;
        }

        public override void Update()
        {
            base.Update();
            if (isEnabled)
            {
                // using delta time here ensures that the speed of change will be the same for different frame rates
                radius += Random.Range(-velocityChange.value, velocityChange.value) * Time.deltaTime;
                angle += Random.Range(-angleChange.value, angleChange.value) * Time.deltaTime;
                
                if (radius > forceRange.max)
                    radius = forceRange.max;
                else if (radius < forceRange.min)
                    radius = forceRange.min;

                float x = radius * Mathf.Cos(angle);
                float z = radius * Mathf.Sin(angle);

                force = new Vector3(x, 0, z);
            }
        }
    }
}