using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOpacity : MonoBehaviour {

    public GameObject waterSurface;
    private bool underwater;
    public Color waterColor = new Color(0.22f, 0.65f, 0.77f, 0.5f);
    public float waterFog = 0.2f;
    public bool dataCollecting = false;

	// Use this for initialization
	void Start () {
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = waterColor;
        RenderSettings.fogDensity = waterFog;
    }
	
	// Update is called once per frame
	void Update () {
        if (((transform.position.y < waterSurface.transform.position.y) != underwater) && (!dataCollecting))
        {
            underwater = (transform.position.y < waterSurface.transform.position.y);
            if (underwater) SetUnderwater();
            else SetNormal();
        }
        if (waterFog != RenderSettings.fogDensity)
            RenderSettings.fogDensity = waterFog;
        if (waterColor != RenderSettings.fogColor)
            RenderSettings.fogColor = waterColor;
	}

    public void SetNormal()
    {
        RenderSettings.fog = false;
    }

    public void SetUnderwater()
    {
        RenderSettings.fog = true;
    }
}
 