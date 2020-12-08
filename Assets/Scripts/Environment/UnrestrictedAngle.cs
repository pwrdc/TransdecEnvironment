using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class UnrestrictedAngle : RandomizedParameter<float>
    {
        float value;

        public override float Value => value;

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