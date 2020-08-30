using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(ParticleSystem)), ExecuteInEditMode]
    public class FogParticles : MonoBehaviour
    {
        public float opacity = 0.2f;
        ParticleSystemRenderer particleSystemRenderer;
        new ParticleSystem particleSystem;
        UnderwaterEffects underwaterEffect;

        // Start is called before the first frame update
        void Start()
        {
            underwaterEffect = FindObjectOfType<UnderwaterEffects>();
            particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
            particleSystem = GetComponent<ParticleSystem>();
        }

        Color TransferAlpha(Color colorSource, Color alphaSource)
        {
            return new Color(colorSource.r, colorSource.g, colorSource.b, alphaSource.a);
        }

        Color ToGrayscale(Color color)
        {
            return new Color(color.grayscale, color.grayscale, color.grayscale, color.a);
        }

        void Update()
        {
            if (underwaterEffect != null && particleSystem != null)
            {
                Color color = underwaterEffect.fogColor.value;
                color.a = opacity;
                if(particleSystemRenderer.sharedMaterial!=null)
                    particleSystemRenderer.sharedMaterial.color= ToGrayscale(color);
                ParticleSystem.MainModule main = particleSystem.main;
                main.startColor = new ParticleSystem.MinMaxGradient(TransferAlpha(color, main.startColor.colorMin), TransferAlpha(color, main.startColor.colorMax));
            }
        }
    }
}