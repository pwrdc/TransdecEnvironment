using System;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;
    [Range(0f, 10f)]
    public float radius = 3.0f;
    public float water_level = 11.0f;
    [Range(0f, 10f)]
    public float x_range = 0.5f;
    [Range(0f, 10f)]
    public float y_range = 0.5f;
    [Range(0f, 10f)]
    public float z_range = 0.5f;
    [Range(0f, 30f)]
    public float x_ang_range = 5f;
    [Range(0f, 30f)]
    public float y_ang_range = 5f;
    [Range(0f, 30f)]
    public float z_ang_range = 5f;

    System.Random rnd = new System.Random();

    float get_random(float min, float max)
    {
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
