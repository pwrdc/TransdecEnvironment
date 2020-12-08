using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Environment
{
    [System.Serializable]
    public class ColorParameter : RandomizedParameter<Color>
    {
        protected Color randomValue;
        public Color normal;
        public Color[] colorPoints;

        public enum PreviewMode
        {
            Normal,
            ColorPointIndex,
            Randomized
        }
        
        public PreviewMode previewMode;
        bool ShowPreviewIndex => previewMode == PreviewMode.ColorPointIndex;

        public override Color Value
        {
            get
            {
                switch (previewMode)
                {
                    case PreviewMode.ColorPointIndex:
                        if (colorPoints.Length > 0)
                            return colorPoints[Mathf.Clamp(previewIndex, 0, colorPoints.Length - 1)];
                        else
                            return normal;
                    case PreviewMode.Normal:
                        return normal;
                    case PreviewMode.Randomized:
                        return randomValue;
                    default:
                        throw new InvalidEnumValueException(previewMode);
                }
                
            }
        }

        [AllowNesting, ShowIf("ShowPreviewIndex")]
        public int previewIndex = 0;

        public override void Randomize()
        {
            // This is basically weighted geometric mean implemented for colors, 
            // it yields much more diverse colors than weighted arithmetic mean.
            // source: https://en.wikipedia.org/wiki/Weighted_geometric_mean
            float r = 1;
            float g = 1;
            float b = 1;
            float weightsSum = 0;
            foreach(var color in colorPoints)
            {
                float weight = Random.value;
                r *= Mathf.Pow(color.r, weight);
                g *= Mathf.Pow(color.g, weight);
                b *= Mathf.Pow(color.b, weight);
                weightsSum += weight;
            }
            float weightsSumInverted = 1f / weightsSum;
            r = Mathf.Pow(r, weightsSumInverted);
            g = Mathf.Pow(g, weightsSumInverted);
            b = Mathf.Pow(b, weightsSumInverted);
            randomValue = new Color(r, g, b);
            previewMode = PreviewMode.Randomized;
        }

        public override void SetAsNormal()
        {
            randomValue = normal;
        }
    }
}