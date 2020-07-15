using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [System.Serializable]
    public abstract class RandomizedParameter
    {
        public abstract void Randomize();
        public abstract void SetAsNormal();
        public abstract void Preview();
    }
}