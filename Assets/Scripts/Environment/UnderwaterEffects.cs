using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Environment
{
    [ExecuteInEditMode]
    public class UnderwaterEffects : MonoBehaviour
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

        PostProcessVolume volume;
        WobbleEffect[] wobbleEffects;

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
            volume = GetComponent<PostProcessVolume>();
            wobbleEffects = FindObjectsOfType<WobbleEffect>();
            Environment.Instance.OnNormalInit+=NormalInit;
            Environment.Instance.OnRandomizedInit+=RandomizedInit;
            RenderSettings.fogMode = FogMode.Exponential;
        }

        bool underwater => target!=null && Environment.Instance.waterSurface !=null && target.position.y < Environment.Instance.waterSurface.position.y;

        void Update()
        {
            foreach(var wobbleEffect in wobbleEffects)
            {
                wobbleEffect.enabled = underwater;
            }
            volume.enabled = underwater;
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