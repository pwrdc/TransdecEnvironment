using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class WaterOpacity : MonoBehaviour
    {
        private Color waterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
        private float waterFog = 0.25f;
        public float WaterFog { get { return waterFog; } }

        private bool underwater;


        void Start()
        {
            RenderSettings.fogMode = FogMode.Exponential;
            RobotAgent.Instance.OnDataEnvironmentValuesUpdate += UpdateData;
        }

        void Update()
        {
            if (waterColor != RenderSettings.fogColor)
                RenderSettings.fogColor = waterColor;
            if (waterFog != RenderSettings.fogDensity)
                RenderSettings.fogDensity = waterFog;
            underwater = (transform.position.y < Environment.Instance.waterSurface.position.y);
            RenderSettings.fog = underwater;
        }

        public void SetWaterColor(Color color) { waterColor = color; }
        public void SetWaterFog(float fog) { waterFog = fog; }

        public void UpdateData(EnvironmentInitValues settings)
        {
            waterColor = settings.normalWaterColor;
            waterFog = settings.normalWaterFog;
        }
    }
}