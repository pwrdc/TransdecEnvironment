using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public abstract class Randomized : MonoBehaviour
    {
        public abstract void InitializeNormal();
        public abstract void InitializeRandom();

        public virtual void Start()
        {
            Environment.Instance.OnNormalInit += InitializeNormal;
            Environment.Instance.OnRandomizedInit += InitializeRandom;
        }

    }
}