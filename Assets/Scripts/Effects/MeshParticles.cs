using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

using PointsSorter = System.Func<UnityEngine.Vector3, float>;

public class MeshParticles : MonoBehaviour
{
    public Particles[] particles;
    List<ParticleSystem> placedParticles = new List<ParticleSystem>();

    [System.Serializable]
    public struct Particles
    {
        public ParticleSystem prefab;
        [Range(0, 1)]
        public float percentage;
        public VertexSelectionType vertexSelectionType;
        public bool useColor;
    }

    [System.Serializable]
    public enum VertexSelectionType
    {
        Farthest,
        Lowest,
        Highest,
        Random
    }
    // maps vertex selection types to their implementations as lambdas taking a point and returning how "favorable" it is
    // there's no point in making it a switch since the access is done at runtime and cannot be inlined anyways
    static readonly Dictionary<VertexSelectionType, PointsSorter> vertexSelectionTypesImplementations =
        new Dictionary<VertexSelectionType, PointsSorter>()
        {
            [VertexSelectionType.Farthest] = v => v.magnitude,
            [VertexSelectionType.Lowest] = v => -v.y,
            [VertexSelectionType.Highest] = v => v.y,
            [VertexSelectionType.Random] = v => Random.value,
        };
    // range of small random value added during points sorting
    static readonly float RANDOMIZATION = 0.1f;
    
    [Button]
    void Regenerate()
    {
        placedParticles.ForEach(p => Destroy(p.gameObject));
        placedParticles.Clear();
        AddParticles();
    }

    void Start()
    {
        AddParticles();
    }


    void AddParticles()
    {
        List<VertexData> vertices = GetVertices();

        // group particles by their selection types (this way vertices will be sorted only once for each type)
        foreach (var grouping in particles.GroupBy(p => p.vertexSelectionType))
        {
            PointsSorter pointsSorter = vertexSelectionTypesImplementations[grouping.Key];
            // add slight randomization because vertices are usually ordered 
            // and we don't want all of the particles to cluster in one place
            float sorter(VertexData vertexData) => pointsSorter(vertexData.position) + Random.value * RANDOMIZATION;

            IEnumerable<VertexData> sortedPoints = vertices.OrderByDescending(sorter);
            foreach (var particles in grouping)
            {
                // create at least one particle system to make this process worthwhile
                int count = Mathf.Max(1, (int)(vertices.Count() * particles.percentage));
                foreach (var (index, vertex) in sortedPoints.Take(count).Enumerate())
                {
                    ParticleSystem instance = Instantiate(particles.prefab, vertex.position, transform.rotation, transform);
                    // make sure that instance's global scale is equal to one since meshes in the project have wildly different scales
                    instance.transform.localScale = transform.InverseTransformVector(Vector3.one);
                    // set particle system color if it is needed and possible
                    if (particles.useColor && vertex.color != null)
                    {
                        ParticleSystem.MainModule main = instance.main;
                        main.startColor = vertex.color.Value;
                    }
                    placedParticles.Add(instance);
                }
            }
        }
    }

    struct VertexData
    {
        public Vector3 position;
        public Color? color;
    }

    List<VertexData> GetVertices()
    {
        List<VertexData> vertices = new List<VertexData>();
        foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = meshFilter.mesh;
            Transform meshTransform = meshFilter.transform;
            Material material = meshFilter.GetComponent<MeshRenderer>()?.material;
            Vector3[] meshVertices = mesh.vertices;
            // add converted vertices to the list
            foreach (var vertex in meshVertices)
            {
                vertices.Add(new VertexData
                {
                    // transform vertices to world space
                    position = meshTransform.TransformPoint(vertex),
                    color = material?.color
                });
            }
        }
        return vertices;
    }
}
