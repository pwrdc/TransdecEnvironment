using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Environment
{

    [ExecuteInEditMode]
    public class UnderwaterEffects : Randomized
    {
        [System.Serializable]
        public class Randomization {
            public float minWaterFog = 0.2f;
            public float maxWaterFog = 0.4f;
            public Gradient fogColor;
            public float normalWaterFog = 0.25f;
            public Color normalWaterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
        }
        public Transform target;
        public Randomization randomization;

        Color waterColor = new Color(0.22f, 0.65f, 0.65f, 0.5f);
        [HideInInspector]
        public float waterFog = 0.25f;
        public Light robotLight;

        // Linear fog mode is for slowest devices, looks unrealistic
        // and requires additional two parameters (start and end)
        // so fog mode is limited to exponential variants
        [System.Serializable]
        public enum ExponentialFogMode
        {
            Exponential, ExponentialSquared
        }
        FogMode ExponentailFogModeToFogMode(ExponentialFogMode fogMode)
        {
            switch (fogMode)
            {
                case ExponentialFogMode.Exponential:
                    return FogMode.Exponential;
                case ExponentialFogMode.ExponentialSquared:
                    return FogMode.ExponentialSquared;
                default:
                    throw new InvalidEnumValueException(fogMode);
            }
        }
        public ExponentialFogMode fogMode=ExponentialFogMode.ExponentialSquared;

        PostProcessVolume volume;
        WobbleEffect[] wobbleEffects;

        public override void InitializeRandom(){
            waterFog = Random.Range(randomization.minWaterFog, randomization.maxWaterFog);
            waterColor = randomization.fogColor.Evaluate(Random.value);
        }

        public override void InitializeNormal(){
            waterColor=randomization.normalWaterColor;
            waterFog=randomization.normalWaterFog;
        }

        public override void Start()
        {
            volume = GetComponent<PostProcessVolume>();
            // wobble effects must be attached directly to each camera
            wobbleEffects = FindObjectsOfType<WobbleEffect>();
            base.Start();
        }

        void OnDisable()
        {
            UpdateEffects(false);
        }

        void OnEnable()
        {
            Start();
            UpdateEffects(true);
        }

        void UpdateEffects(bool active)
        {
            foreach (var wobbleEffect in wobbleEffects)
            {
                wobbleEffect.enabled = active;
            }
            if (volume != null)
            {
                // Unity's implementation of PostProcessVolume is buggy
                // and disabling volume through enabled causes bunch of NullPointerExceptions
                volume.weight = active ? 1 : 0;
            }
            if (robotLight != null) robotLight.enabled = active;
            RenderSettings.fog = active;
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFog;
            RenderSettings.fogMode = ExponentailFogModeToFogMode(fogMode);

            // preview water opacity in the edit mode
            if (Application.isEditor && !Application.isPlaying)
            {
                InitializeNormal();
            }
        }

        void Update()
        {
            bool underwater = target != null && Environment.Instance.waterSurface != null && target.position.y < Environment.Instance.waterSurface.position.y;
            UpdateEffects(underwater);
        }
    }
}