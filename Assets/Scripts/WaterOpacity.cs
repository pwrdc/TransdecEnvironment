using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOpacity : MonoBehaviour {

    public GameObject water_surface;
    private bool underwater;
    public Color water_color = new Color(0.22f, 0.65f, 0.77f, 0.5f);
    public float water_fog = 0.2f;
    public bool data_collecting = false;

	// Use this for initialization
	void Start () {
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = water_color;
        RenderSettings.fogDensity = water_fog;
    }
	
	// Update is called once per frame
	void Update () {
        if (((transform.position.y < water_surface.transform.position.y) != underwater) && (!data_collecting))
        {
            underwater = (transform.position.y < water_surface.transform.position.y);
            if (underwater) SetUnderwater();
            else SetNormal();
        }
	}

    public void SetNormal()
    {
        RenderSettings.fog = false;
        Debug.Log("Normal");
    }

    public void SetUnderwater()
    {
        RenderSettings.fog = true;
        Debug.Log("Underwater");
    }
}
 