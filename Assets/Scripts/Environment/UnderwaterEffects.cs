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
        public float waterLevelOffset = -1f;

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

        public override void Start()
        {
            base.Start();
            parameters.Add(filterColor);
            parameters.Add(fogColor);
            parameters.Add(fogDensity);
            parameters.Add(volumetricDensity);
        }

        void OnDisable()
        {
            UpdateEffects(0f);
        }

        void OnEnable()
        {
            Start();
            UpdateEffects(1f);
        }

        float Cube(float x)
        {
            return x * x *x;
        }

        float CalculateEffectsWeight()
        {
            Camera camera = RobotAgent.Instance.ActiveCamera;
            if (camera == null)
                return 1f;
            Vector3 screenBottom = camera.ViewportToWorldPoint(new Vector3(0.5f, 0, camera.nearClipPlane));
            Vector3 screenTop = camera.ViewportToWorldPoint(new Vector3(0.5f, 1, camera.nearClipPlane));
            return Cube(Mathf.InverseLerp(screenBottom.y, screenTop.y, WaterSurface.Y + waterLevelOffset));
        }

        void UpdateEffects(float weight)
        {
            float activeThreshold = 0.1f;
            bool active = weight > activeThreshold;
            if (postProcessVolume != null)
            {
                // Unity's implementation of PostProcessVolume is buggy
                // and disabling volume through enabled causes bunch of NullPointerExceptions
                postProcessVolume.weight = weight;
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

                VaryingBlur varyingBlur = postProcessVolume.sharedProfile.GetSetting<VaryingBlur>();
                varyingBlur.Color.value = fogColor.value;
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
            bool underwater = target != null && WaterSurface.IsAbove(target.position.y);
            UpdateEffects(CalculateEffectsWeight());
        }
    }
}