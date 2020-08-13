using UnityEngine;
using System.Collections.Generic;

public class Pinger : MonoBehaviour
{
    static Dictionary<float, List<Pinger>> pingers=new Dictionary<float, List<Pinger>>();
    // zero frequency is reserved as null value
    public float frequency;

    private void OnEnable()
    {
        if (frequency != 0.0)
        {
            if(!pingers.ContainsKey(frequency))
                pingers.Add(frequency, new List<Pinger>());
            pingers[frequency].Add(this);
        }
    }

    private void OnDisable()
    {
        if (frequency != 0.0)
            pingers.Remove(frequency);
    }

    public static List<Pinger> FindPingers(float frequency)
    {
        List<Pinger> result;
        pingers.TryGetValue(frequency, out result);
        return result;
    }
}
