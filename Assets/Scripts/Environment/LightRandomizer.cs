using UnityEngine;

namespace Environment
{
    [ExecuteInEditMode]
    public class LightRandomizer : Randomized
    {
        public IntParameter angle = new IntParameter(80, 60, 120);
        public FloatParameter intensity = new FloatParameter(1.2f, 0.5f, 2f);
        new Light light;
        float yAngle;

        public override void InitializeRandom()
        {
            base.InitializeRandom();
            yAngle = Random.Range(0, 360);
        }

        public override void InitializeNormal()
        {
            base.InitializeNormal();
            yAngle = 0;
        }

        public override void Start()
        {
            base.Start();
            parameters.Add(angle);
            parameters.Add(intensity);
            light = GetComponent<Light>();
        }

        public override void Update()
        {
            base.Update();
            light.intensity = intensity.value;
            light.transform.rotation = Quaternion.Euler(angle.value, yAngle, 0);
        }
    }
}