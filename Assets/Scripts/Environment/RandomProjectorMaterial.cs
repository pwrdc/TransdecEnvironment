using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projector))]
[RequireComponent(typeof(Placeable))]
public class RandomProjectorMaterial : MonoBehaviour
{
    public Material[] materials;
    Projector projector;

    void Start()
    {
        projector = GetComponent<Projector>();
        GetComponent<Placeable>().OnPlaced.AddListener(RandomizeMaterial);
        RandomizeMaterial();
    }

    void RandomizeMaterial()
    {
        projector.material = Utils.RandomChoice(materials);
    }
}
