using UnityEngine;

namespace Environment
{
    public class LightManager : MonoBehaviour
    {
        private new Light light;

        private void Start()
        {
            light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        }

        public void initializeLight(float angle, float intensitivity)
        {
            light.intensity = intensitivity;
            light.transform.rotation = Quaternion.Euler(angle, -90, 0);
        }
    }
}