using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Environment
{
    [ExecuteInEditMode]
    public class UnderwaterEffects : Randomized
    {
        public ColorEnvironmentParameter filterColor;
        public ColorEnvironmentParameter fogColor;
        public FloatParameter fogDensity=new FloatParameter(0.085f, 0.05f, 0.1f);
        public FloatParameter volumetricDensity=new FloatParameter(2, 1, 5);
        public Aura2API.AuraVolume auraVolume;

        public Transform target;
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
            fogDensity.Randomize();
            fogColor.Randomize();
            filterColor.Randomize();
            volumetricDensity.Randomize();
        }

        public override void InitializeNormal(){
            fogDensity.SetAsNormal();
            fogColor.SetAsNormal();
            filterColor.SetAsNormal();
            volumetricDensity.SetAsNormal();
        }

        public void Preview()
        {
            fogDensity.Preview();
            fogColor.Preview();
            filterColor.Preview();
            volumetricDensity.Preview();
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
            RenderSettings.fogColor = fogColor.value;
            RenderSettings.fogDensity = fogDensity.value;
            RenderSettings.fogMode = ExponentailFogModeToFogMode(fogMode);
            if (auraVolume != null)
            {
                auraVolume.densityInjection.strength = volumetricDensity.value;
                auraVolume.scatteringInjection.strength = volumetricDensity.value / 10;
            }
            ColorGrading colorGrading = volume.sharedProfile.GetSetting<ColorGrading>();
            colorGrading.colorFilter.value = filterColor.value;

            // preview water opacity in the edit mode
            if (Application.isEditor && !Application.isPlaying)
            {
                InitializeNormal();
            }
        }

        void Update()
        {
            bool underwater = target != null && Environment.Instance.waterSurface != null && target.position.y < Environment.Instance.waterSurface.position.y;
            if (!Application.isPlaying)
                Preview();
            UpdateEffects(underwater);
        }
    }
}