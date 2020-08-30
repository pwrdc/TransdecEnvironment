using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyForce : MonoBehaviour
{
    static BuoyancyForce instance;
    public static BuoyancyForce Instance => Singleton.GetInstance(ref instance);

    public float positionStep = 1f;
    public float timeStep = 1f;
    public float waterDensity=997f;
    public float noiseInfluence = 0.1f;

    [System.Serializable]
    public class Visualisation
    {
        public int gridSize=10;
        public float pointsOffset = 10f;
        public float referenceVolume = 0.01f;
    }
    public Visualisation visualisation;

    void Start()
    {
        Singleton.Initialize(this, ref instance);
    }

    /// <summary>
    /// Returns buoyancy force fluctuations at given postion.
    /// </summary>
    /// <param name="position">World position of the point.</param>
    /// <returns>Force in range -1 to 1,</returns>
    public float GetFluctuations(Vector3 position)
    {
        return Perlin.Noise(position.x * positionStep, position.z * positionStep, Time.time * timeStep);
    }

    // transforms noise value from range <-1, 1> to <0, 1>
    float ToZeroOneRange(float value)
    {
        return value = (1 + value) / 2;
    }

    /// <summary>
    /// Returns buayancy force for a body located in water.
    /// </summary>
    /// <param name="position">World position of the body.</param>
    /// <param name="volume">Volume of the body.</param>
    /// <returns></returns>
    public float GetForce(Vector3 position, float volume)
    {
        return Mathf.Lerp(1f, ToZeroOneRange(GetFluctuations(position)), noiseInfluence) * volume * waterDensity * (-Physics.gravity.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        int halfGridSize = visualisation.gridSize / 2;
        for (int x=-halfGridSize; x< halfGridSize; x++)
        {
            for (int z = -halfGridSize; z < halfGridSize; z++)
            {
                Vector3 position = new Vector3(x, 0, z) * visualisation.pointsOffset;
                Gizmos.DrawRay(transform.position + position, Vector3.up*GetForce(position, visualisation.referenceVolume));
            }
        }
    }
}
