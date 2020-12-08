using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public abstract class Randomized : MonoBehaviour
    {
        protected List<RandomizedParameter> parameters=new List<RandomizedParameter>();
        public virtual void InitializeNormal()
        {
            foreach(var parameter in parameters)
            {
                parameter.SetAsNormal();
            }
        }
        public virtual void InitializeRandom()
        {
            foreach (var parameter in parameters)
            {
                parameter.Randomize();
            }
            ApplyParameters();
        }

        public virtual void ApplyParameters()
        {

        }

        public virtual void Start()
        {
            Environment.Instance.OnNormalInit += InitializeNormal;
            Environment.Instance.OnRandomizedInit += InitializeRandom;
        }
        
        public virtual void Update()
        {
            ApplyParameters();
        }
    }
}