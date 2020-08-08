using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [System.Serializable]
    public class ColorParameter : RandomizedParameter
    {
        [HideInInspector]
        public Color value;
        public Color normal;
        public Color[] colorPoints;

        public enum PreviewMode
        {
            Normal,
            ColorPointIndex
        }

        [Header("Edit Mode Preview")]
        public PreviewMode previewMode;
        public int previewIndex = 0;

        public override void Preview()
        {
            if (colorPoints.Length > 0 && previewMode == PreviewMode.ColorPointIndex)
                value = colorPoints[Mathf.Clamp(previewIndex, 0, colorPoints.Length - 1)];
            else
                value = normal;
        }

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
            value = new Color(r, g, b);
        }

        public override void SetAsNormal()
        {
            value = normal;
        }
    }
}