using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Environment
{
    [System.Serializable]
    public abstract class LinearParameter<T> : RandomizedParameter<T>
    {
        protected T randomValue;

        public override T Value {
            get {
                switch (previewMode)
                {
                    case PreviewMode.Min:
                        return min;
                    case PreviewMode.Max:
                        return max;
                    case PreviewMode.Normal:
                        return normal;
                    case PreviewMode.Percentage:
                        return Blend((float)previewPercentage / 100);
                    case PreviewMode.Randomized:
                        return randomValue;
                    default:
                        throw new InvalidEnumValueException(previewMode);
                }
            }
        }

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
            Percentage,
            Randomized
        }
        
        public PreviewMode previewMode;
        bool ShowPreviewPercentage => previewMode == PreviewMode.Percentage;
        [ShowIf("ShowPreviewPercentage"), MinValue(0), MaxValue(100), AllowNesting]
        public int previewPercentage;

        public override void Randomize()
        {
            randomValue = Blend(Random.value);
            previewMode = PreviewMode.Randomized;
        }

        public override void SetAsNormal()
        {
            previewMode = PreviewMode.Normal;
        }

        public abstract T Blend(float percentage);
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
