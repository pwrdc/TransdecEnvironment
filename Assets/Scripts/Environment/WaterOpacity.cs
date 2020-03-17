using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [ExecuteInEditMode]
    public class WaterOpacity : MonoBehaviour
    {

        [System.Serializable]
        public class Randomization {
            public float minWaterFog = 0.2f;
            public float maxWaterFog = 0.4f;
            public Vector3 minWaterHSV = new Vector3(180, 0, 50);
            public Vector3 maxWaterHSV = new Vector3(250, 100, 100);
            public float normalWaterFog = 0.25f;
            public Color normalWaterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
        }
        public Transform target;
        public Randomization randomization;
        Color waterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
        [HideInInspector]
        public float waterFog = 0.25f;

        public void RandomizedInit(){
            float percentageIntensitivity = Random.value;
            waterFog = randomization.minWaterFog + (percentageIntensitivity * (randomization.maxWaterFog - randomization.minWaterFog));
            float h = Utils.GetRandom(randomization.minWaterHSV.x, randomization.maxWaterHSV.x) / 360;
            float s = Utils.GetRandom(randomization.minWaterHSV.y, randomization.maxWaterHSV.y) / 100;
            float v = Utils.GetRandom(randomization.minWaterHSV.z, randomization.maxWaterHSV.z) / 100;
            Color rgb = Color.HSVToRGB(h, s, v);
            waterColor=rgb;
        }

        public void NormalInit(){
            waterColor=randomization.normalWaterColor;
            waterFog=randomization.normalWaterFog;
        }

        void Start()
        {
            Environment.Instance.OnNormalInit+=NormalInit;
            Environment.Instance.OnRandomizedInit+=RandomizedInit;
            RenderSettings.fogMode = FogMode.Exponential;
            
        }

        void Update()
        {
            Environment environment=Environment.Instance;
            bool underwater = (target.position.y < environment.waterSurface.position.y);
            RenderSettings.fog = underwater;
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFog;

            // preview water opacity in the edit mode
            if(Application.isEditor && !Application.isPlaying)
            {
                NormalInit();
            }
        }
    }
}