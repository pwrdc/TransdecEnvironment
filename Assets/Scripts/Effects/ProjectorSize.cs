using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class ProjectorSize : MonoBehaviour
{
    Projector projector;

    void Start()
    {
        projector = GetComponent<Projector>();
    }
    
    void Update()
    {
        Texture texture = projector.material.GetTexture("_ShadowTex");
        projector.aspectRatio = texture.width / texture.height;
    }
}
