using UnityEngine;

namespace Environment
{
    [ExecuteInEditMode]
    public class LightRandomizer : Randomized
    {
        public IntParameter angle = new IntParameter(80, 60, 120);
        public FloatParameter intensity = new FloatParameter(1.2f, 0.5f, 2f);
        new private Light light;

        public override void Start()
        {
            base.Start();
            light = GetComponent<Light>();
        }

        public override void InitializeNormal(){
            angle.SetAsNormal();
            intensity.SetAsNormal();
        }

        public override void InitializeRandom(){
            angle.Randomize();
            intensity.Randomize();
        }

        public void Preview()
        {
            angle.Preview();
            intensity.Preview();
        }

        public void Update()
        {
            if (!Application.isPlaying)
                Preview();
            light.intensity = intensity.value;
            light.transform.rotation = Quaternion.Euler(angle.value, -90, 0);
        }
    }
}