using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class UnrestrictedAngle : RandomizedParameter
    {
        public float value;
        public override void Preview()
        {
            value = 0f;
        }

        public override void Randomize()
        {
            value = Random.Range(0, 360);
        }

        public override void SetAsNormal()
        {
            value = 0f;
        }
    }
}