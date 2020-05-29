using UnityEngine;

namespace Environment
{
    public class LightRandomizer : Randomized
    {
        public int normalLightAngle = 80;
        public float normalLightIntensivity = 0.6f;
        private Light light;
        public int minLightAngle = 60;
        public int maxLightAngle = 120;
        [Range(0.0f, 0.3f)]
        public float minIntensivity = 0.1f;
        [Range(0.3f, 1f)]
        public float maxIntensivity = 1f;

        public override void Start()
        {
            base.Start();
            light = GetComponent<Light>();
        }

        public override void InitializeNormal(){
            InitializeLight(normalLightAngle, normalLightIntensivity);
        }

        public override void InitializeRandom(){
            float angle = Utils.GetRandom(minLightAngle, maxLightAngle);
            float intensitivity = Utils.GetRandom(minIntensivity, maxIntensivity);
            InitializeLight(angle, intensitivity);
        }

        public void InitializeLight(float angle, float intensitivity)
        {
            light.intensity = intensitivity;
            light.transform.rotation = Quaternion.Euler(angle, -90, 0);
        }
    }
}