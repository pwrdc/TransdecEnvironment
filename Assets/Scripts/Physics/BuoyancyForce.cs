using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyForce : MonoBehaviour
{
    //Singleton
    private static BuoyancyForce mInstance = null;
    public static BuoyancyForce Instance =>
        mInstance == null ? (mInstance = FindObjectOfType<BuoyancyForce>()) : mInstance;

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

    public float GetForce(Vector3 position, float volume)
    {
        float noiseValue = Perlin.Noise(position.x * positionStep, position.z * positionStep, Time.time * timeStep);
        // transform noise value from range <-1, 1> to <0, 1>
        noiseValue=(1+noiseValue)/ 2;
        // float waterColumnHeight = waterSurfaceY + noiseValue - position.y;
        return Mathf.Lerp(1f, noiseValue, noiseInfluence) * volume * waterDensity*(-Physics.gravity.y);
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
