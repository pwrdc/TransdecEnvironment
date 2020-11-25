using System.Collections.Generic;
using UnityEngine;

public class MeshParticles : MonoBehaviour
{
    List<Vector3> points=new List<Vector3>();
    [Range(0, 1)]
    public float pointsPercentage = 0.3f;
    Transform meshTransform;

    public Transform[] particlePrefabs;

    void Start()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        meshTransform = meshFilter.transform;
        Mesh mesh = meshFilter.mesh;
        foreach(var vertex in mesh.vertices)
        {
            if (Random.value < pointsPercentage)
            {
                points.Add(vertex);
            }
        }
        foreach (var point in points)
        {
            Transform particles=Instantiate(Utils.RandomChoice(particlePrefabs), meshTransform.TransformPoint(point), meshTransform.rotation, meshTransform);
            particles.localScale = meshTransform.InverseTransformVector(Vector3.one);
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var point in points)
        {
            Gizmos.DrawSphere(meshTransform.TransformPoint(point), 0.01f);
        }
    }
}
