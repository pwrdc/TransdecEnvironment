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
    }

    [System.Serializable]
    public abstract class RandomizedParameter<T> : RandomizedParameter
    {
        public abstract T Value { get; }
    }
}