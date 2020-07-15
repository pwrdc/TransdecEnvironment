using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [System.Serializable]
    public class ColorEnvironmentParameter : EnvironmentParameter<Color>
    {
        public Color normal;
        public Color[] colorPoints;
        public int previewIndex = 0;
        public enum PreviewMode
        {
            Normal,
            ColorPointIndex
        }
        public PreviewMode previewMode;

        public override void Preview()
        {
            if (colorPoints.Length > 0 && previewMode == PreviewMode.ColorPointIndex)
                value = colorPoints[Mathf.Clamp(previewIndex, 0, colorPoints.Length - 1)];
            else
                value = normal;
        }

        public override void Randomize()
        {
            Color accumulator=normal;
            foreach(var color in colorPoints)
            {
                accumulator = Color.Lerp(accumulator, color, Random.value);
            }
            value = accumulator;
        }

        public override void SetAsNormal()
        {
            value = normal;
        }
    }
}