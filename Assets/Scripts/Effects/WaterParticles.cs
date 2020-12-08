using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace Environment {
    /// <summary>
    /// Manages water particles attached to the robot.
    /// Allows randomizing water particles spawn rate
    /// and switches them between local and world space
    /// according to simulation mode.
    /// </summary>
    [ExecuteInEditMode]
    public class WaterParticles : Randomized
    {
        public FloatParameter emissionRate=new FloatParameter(200, 50, 300);
        new ParticleSystem particleSystem;
        [ResetParameter] bool collectData = false;

        public override void Start()
        {
            ResetParameterAttribute.InitializeAll(this);
            base.Start();
            parameters.Add(emissionRate);
            particleSystem = GetComponent<ParticleSystem>();
        }

        public override void Update()
        {
            ParticleSystem.MainModule settings=particleSystem.main;

            // In data collection mode the camera is jumping around every frame
            // so the world space particles would just stay behind it.
            settings.simulationSpace = collectData ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;

            // update emission rate inside of the particle system
            ParticleSystem.EmissionModule emission=particleSystem.emission;
            emission.rateOverTime = emissionRate.Value;
            // bursts count is same as main emission rate
            for(int i=0; i<emission.burstCount; i++) {
                ParticleSystem.Burst burst=emission.GetBurst(i);
                burst.count = emissionRate.Value;
                emission.SetBurst(i, burst);
            }
            base.Update();
        }
    }
}