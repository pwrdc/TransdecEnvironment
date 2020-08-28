using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Environment
{
    [ExecuteInEditMode]
    public class UnderwaterEffects : Randomized
    {
        public ColorParameter filterColor;
        public ColorParameter fogColor;
        public FloatParameter fogDensity=new FloatParameter(0.085f, 0.05f, 0.1f);
        public FloatParameter volumetricDensity=new FloatParameter(2, 1, 5);
        public Aura2API.AuraVolume auraVolume;
        public PostProcessVolume postProcessVolume;

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

        WobbleEffect[] wobbleEffects;

        public override void Start()
        {
            base.Start();
            parameters.Add(filterColor);
            parameters.Add(fogColor);
            parameters.Add(fogDensity);
            parameters.Add(volumetricDensity);
            // wobble effects must be attached directly to each camera
            wobbleEffects = FindObjectsOfType<WobbleEffect>();
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
            if (wobbleEffects != null)
            {
                foreach (var wobbleEffect in wobbleEffects)
                {
                    wobbleEffect.enabled = active;
                }
            }
            if (postProcessVolume != null)
            {
                // Unity's implementation of PostProcessVolume is buggy
                // and disabling volume through enabled causes bunch of NullPointerExceptions
                postProcessVolume.weight = active ? 1 : 0;
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
            if (postProcessVolume != null)
            {
                ColorGrading colorGrading = postProcessVolume.sharedProfile.GetSetting<ColorGrading>();
                colorGrading.colorFilter.value = filterColor.value;
            }

            // preview water opacity in the edit mode
            if (Application.isEditor && !Application.isPlaying)
            {
                InitializeNormal();
            }
        }

        public override void Update()
        {
            base.Update();
            bool underwater = target != null && Environment.Instance.isUnderwater(target.position.y);
            UpdateEffects(underwater);
        }
    }
}