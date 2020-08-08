using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [System.Serializable]
    public abstract class LinearParameter<T> : RandomizedParameter
    {
        [HideInInspector]
        public T value;
        
        public T normal;
        public T min;
        public T max;

        public LinearParameter(T normal, T min, T max)
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

        [Header("Edit Mode Preview")]
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
    public class FloatParameter : LinearParameter<float>
    {
        public FloatParameter(float normal, float min, float max) : base(normal, min, max) { }

        public override float Blend(float percentage)
        {
            return Mathf.Lerp(min, max, percentage);
        }
    }

    [System.Serializable]
    public class IntParameter : LinearParameter<int>
    {
        public IntParameter(int normal, int min, int max) : base(normal, min, max) { }

        public override int Blend(float percentage)
        {
            return (int)Mathf.Lerp(min, max, percentage);
        }
    }
}
