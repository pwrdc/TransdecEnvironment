using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public int width = 1000;
    public int height = 1000;
    public float scale = 10;

    void Start()
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for(int i=0; i<width; i++)
        {
            for (int j= 0; j < height; j++)
            {
                float noiseValue = Mathf.PerlinNoise(i * scale, j * scale);
                if (noiseValue > 0.5) {
                    texture.SetPixel(i, j, new Color(noiseValue, noiseValue, noiseValue));
                } else
                {
                    texture.SetPixel(i, j, Color.clear);
                }
            }
        }

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
