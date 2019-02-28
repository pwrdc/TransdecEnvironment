using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOpacity : MonoBehaviour {

    public GameObject waterSurface;
    private bool underwater;
    public Color waterColor = new Color(0.22f, 0.65f, 0.77f, 0.5f);
    public float waterFog = 0.2f;

	void Start () {
        RenderSettings.fogMode = FogMode.Exponential;
    }
	
	void Update () {
        if (waterColor != RenderSettings.fogColor)
            RenderSettings.fogColor = waterColor;
        if (waterFog != RenderSettings.fogDensity)
            RenderSettings.fogDensity = waterFog;
        if ((transform.position.y < waterSurface.transform.position.y) != underwater) {
            underwater = (transform.position.y < waterSurface.transform.position.y);
            if (underwater) SetUnderwater();
            else SetNormal();
        }
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
 