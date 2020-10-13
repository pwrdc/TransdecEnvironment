using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [ExecuteInEditMode]
    public class LightCopier : MonoBehaviour
    {
        new Light light;
        public Light copied;
        public float intensityMultiplier = 0.5f;

        void Start()
        {
            light = GetComponent<Light>();
        }
    
        void Update()
        {
            if (light != null)
            {
                light.intensity = copied.intensity * intensityMultiplier;
                transform.rotation = copied.transform.rotation;
            }
        }
    }
}
