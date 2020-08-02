using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(ParticleSystem)), ExecuteInEditMode]
    public class FogParticles : MonoBehaviour
    {
        public float opacity = 0.2f;
        new ParticleSystemRenderer particleSystem;
        UnderwaterEffects underwaterEffect;

        // Start is called before the first frame update
        void Start()
        {
            underwaterEffect = FindObjectOfType<UnderwaterEffects>();
            particleSystem = GetComponent<ParticleSystemRenderer>();
        }

        void Update()
        {
            if (underwaterEffect != null && particleSystem != null)
            {
                Color color = underwaterEffect.fogColor.value;
                color.a = opacity;
                if(particleSystem.sharedMaterial!=null)
                    particleSystem.sharedMaterial.color=color;
            }
        }
    }
}