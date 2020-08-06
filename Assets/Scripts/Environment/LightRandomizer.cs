using UnityEngine;

namespace Environment
{
    [ExecuteInEditMode]
    public class LightRandomizer : Randomized
    {
        public IntParameter xAngle = new IntParameter(80, 60, 120);
        public FloatParameter intensity = new FloatParameter(1.2f, 0.5f, 2f);
        UnrestrictedAngle yAngle=new UnrestrictedAngle();
        new Light light;

        public override void Start()
        {
            base.Start();
            parameters.Add(xAngle);
            parameters.Add(intensity);
            parameters.Add(yAngle);
            light = GetComponent<Light>();
        }

        public override void Update()
        {
            base.Update();
            light.intensity = intensity.value;
            light.transform.rotation = Quaternion.Euler(xAngle.value, yAngle.value, 0);
        }
    }
}