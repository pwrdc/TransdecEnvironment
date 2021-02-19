using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSLabel : MonoBehaviour
{
    public float measurmentPeriod = 2f;

    Text text;
    int frames = 0;
    float nextMeasurment = 0;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        frames++;
        if (Time.time >= nextMeasurment)
        {
            text.text = (frames / measurmentPeriod).ToString();
            nextMeasurment = Time.time + measurmentPeriod;
            frames = 0;
        }
    }
}