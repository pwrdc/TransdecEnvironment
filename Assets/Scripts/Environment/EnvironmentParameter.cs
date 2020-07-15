using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [System.Serializable]
    public abstract class EnvironmentParameter<T>
    {
        [HideInInspector]
        public T value;

        public abstract void Randomize();
        public abstract void SetAsNormal();
        public abstract void Preview();
    }

    [System.Serializable]
    public abstract class LinearEnvironmentParameter<T> : EnvironmentParameter<T>
    {
        public T normal;
        public T min;
        public T max;

        public LinearEnvironmentParameter(T normal, T min, T max)
        {
            this.normal = normal;
            this.min = min;
            this.max = max;
        }

        [System.Serializable]
        public enum PreviewMode
        {
            Min,
            Normal,
            Max,
            Percentage
        }
        public PreviewMode previewMode;
        public float previewPercentage;

        public override void Randomize()
        {
            value = Blend(Random.value);
        }

        public override void SetAsNormal()
        {
            value = normal;
        }

        public abstract T Blend(float percentage);

        public override void Preview()
        {
            switch (previewMode)
            {
                case PreviewMode.Min:
                    value = min;
                    break;
                case PreviewMode.Max:
                    value = max;
                    break;
                case PreviewMode.Normal:
                    value = normal;
                    break;
                case PreviewMode.Percentage:
                    value = Blend(previewPercentage);
                    break;
                default:
                    throw new InvalidEnumValueException(previewMode);
            }
        }
    }

    [System.Serializable]
    public class FloatParameter : LinearEnvironmentParameter<float>
    {
        public FloatParameter(float normal, float min, float max) : base(normal, min, max) { }

        public override float Blend(float percentage)
        {
            return Mathf.Lerp(min, max, percentage);
        }
    }

    [System.Serializable]
    public class IntParameter : LinearEnvironmentParameter<int>
    {
        public IntParameter(int normal, int min, int max) : base(normal, min, max) { }

        public override int Blend(float percentage)
        {
            return (int)Mathf.Lerp(min, max, percentage);
        }
    }
}