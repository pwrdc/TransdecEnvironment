using UnityEngine;
using System.Collections.Generic;

public class Pinger : MonoBehaviour
{
    static Dictionary<float, Pinger> pingers=new Dictionary<float, Pinger>();
    public float frequency;

    private void OnEnable()
    {
        if(frequency!=0.0)
            pingers[frequency]=this;
    }

    private void OnDisable()
    {
        if (frequency != 0.0)
            pingers.Remove(frequency);
    }

    public static Pinger FindPinger(float frequency)
    {
        Pinger result;
        pingers.TryGetValue(frequency, out result);
        return result;
    }
}
