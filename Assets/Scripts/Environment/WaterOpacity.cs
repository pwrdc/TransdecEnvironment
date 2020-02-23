using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneEnvironment
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
            if ((transform.position.y < RobotAgent.Instance.EnvironmentSettings.WaterSurface.transform.position.y) != underwater)
            {
                underwater = (transform.position.y < RobotAgent.Instance.EnvironmentSettings.WaterSurface.transform.position.y);
                if (underwater) SetUnderwater();
                else SetNormal();
            }
        }

        public void SetWaterColor(Color color) { waterColor = color; }
        public void SetWaterFog(float fog) { waterFog = fog; }

        private void SetNormal()
        {
            RenderSettings.fog = false;
        }

        private void SetUnderwater()
        {
            RenderSettings.fog = true;
        }

        public void UpdateData(EnvironmentInitValues settings)
        {
            waterColor = settings.normalWaterColor;
            waterFog = settings.normalWaterFog;
        }
    }
}