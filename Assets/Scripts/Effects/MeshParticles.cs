using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshParticles : MonoBehaviour
{
    [System.Serializable]
    public enum VertexSelectionType
    {
        Farthest,
        Lowest,
        Highest,
        Random
    }
    [System.Serializable]
    public struct Particles
    {
        public Transform prefab;
        [Range(0, 1)]
        public float percentage;
        public VertexSelectionType vertexSelectionType;
    }
    public Particles[] particles;

    Transform meshTransform;

    // there's no point in making it a switch since the access is done at runtime and cannot be inlined anyways
    static readonly Dictionary<VertexSelectionType, System.Func<Vector3, float>> vertexSelectionTypesImplementations = 
        new Dictionary<VertexSelectionType, System.Func<Vector3, float>>()
        {
            [VertexSelectionType.Farthest] = v => v.magnitude,
            [VertexSelectionType.Lowest] = v => -v.y,
            [VertexSelectionType.Highest] = v => v.y,
            [VertexSelectionType.Random] = v => Random.value,
        };

    void Start()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        meshTransform = meshFilter.transform;
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        // group particles by their selection types (this way vertices will be sorted only once for each type)
        foreach(var grouping in particles.GroupBy(p => p.vertexSelectionType))
        {
            IEnumerable<Vector3> sortedPoints = vertices.OrderByDescending(vertexSelectionTypesImplementations[grouping.Key]);
            foreach(var particles in grouping)
            {
                // we will create at least one particle system to make this process wortwhile
                int count = Mathf.Max(1, (int)(vertices.Count() * particles.percentage));
                foreach(var point in sortedPoints.Take(count)) {
                    Transform instance = Instantiate(particles.prefab, meshTransform.TransformPoint(point), meshTransform.rotation, meshTransform);
                    // make sure that instance's global scale is equal to one since mesh's in the project have wildly different scales
                    instance.localScale = meshTransform.InverseTransformVector(Vector3.one);
                }
            }
        }
    }
}
