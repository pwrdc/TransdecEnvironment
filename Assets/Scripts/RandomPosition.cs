using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;
    public GameObject toAnnotate = null;
    public GameObject noiseFolder = null;
    public int numberOfNoiseToGenerate = 5;
    private List<GameObject> otherObjs = new List<GameObject>();

    [System.Serializable]
    public class Gate {
        public float minPhi = -180.0f;
        public float maxPhi = 180.0f;
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        public float waterLevel = 11.0f;
        [Range(0.0f, 11.0f)]
        public float maxDepth = 4.0f;
        [Range(0.0f, 30.0f)]
        public float xAngRange = 15f;
        [Range(0.0f, 30.0f)]
        public float yAngRange = 15f;
        [Range(0.0f, 30.0f)]
        public float zAngRange = 15f;
        //other objects parameters
        [Range(0.0f, 10.0f)]
        public float othMinRadius = 0.0f;
        [Range(0.0f, 10.0f)]
        public float othMaxRadius = 4.0f;
        [Range(0.0f, 11.0f)]
        public float othMaxDepth = 8.0f;
        [Range(0.0f, 90.0f)]
        public float othXAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othYAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othZAngRange = 90f;
    }

    [System.Serializable]
    public class Path {
        public float minPhi = -180.0f;
        public float maxPhi = 180.0f;
        [Range(30.0f, 90.0f)]
        public float cameraFov = 40.0f;
        public float waterLevel = 11.0f;
        [Range(0.0f, 11.0f)]
        public float maxDepth = 9.0f;
        [Range(0.0f, 30.0f)]
        public float xAngRange = 5f;
        [Range(0.0f, 30.0f)]
        public float yAngRange = 5f;
        [Range(0.0f, 30.0f)]
        public float zAngRange = 5f;
        //other objects parameters
        [Range(0.0f, 10.0f)]
        public float othMinRadius = 0.0f;
        [Range(0.0f, 10.0f)]
        public float othMaxRadius = 4.0f;
        [Range(0.0f, 11.0f)]
        public float othMaxDepth = 6.0f;
        [Range(0.0f, 90.0f)]
        public float othXAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othYAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othZAngRange = 90f;
    }

    [System.Serializable]
    public class Bouy_1 {
        public float minPhi = -30.0f;
        public float maxPhi = 30.0f;
        [Range(0.0f, 10.0f)]
        public float minRadius = 3.0f;
        [Range(0.0f, 10.0f)]
        public float maxRadius = 10.0f;
        public float waterLevel = 11.0f;
        [Range(0.0f, 11.0f)]
        public float maxDepth = 9.0f;
        [Range(0.0f, 30.0f)]
        public float xAngRange = 5f;
        [Range(0.0f, 30.0f)]
        public float yAngRange = 5f;
        [Range(0.0f, 30.0f)]
        public float zAngRange = 5f;
        //other objects parameters
        [Range(0.0f, 10.0f)]
        public float othMinRadius = 0.0f;
        [Range(0.0f, 10.0f)]
        public float othMaxRadius = 4.0f;
        [Range(0.0f, 11.0f)]
        public float othMaxDepth = 6.0f;
        [Range(0.0f, 90.0f)]
        public float othXAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othYAngRange = 90f;
        [Range(0.0f, 90.0f)]
        public float othZAngRange = 90f;
    }

    public Gate gate;
    public Path path;
    public Bouy_1 bouy_1;
    public dynamic mode;

    [HideInInspector]
    public Bounds targetBounds;

    float GetRandom(float min, float max)
    {
        System.Random rnd = transform.gameObject.GetComponent<RandomInit>().GetRandomizer();
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    public void Start() {
    	foreach (Transform child in noiseFolder.transform) {
    		otherObjs.Add(child.gameObject);
        }
    }

    public void GetNewPos()
    {
        Vector3 newPos = targetBounds.center;
        float xRot = GetRandom(-mode.xAngRange, mode.xAngRange);
        float yRot = GetRandom(-mode.yAngRange, mode.yAngRange);
        float zRot = GetRandom(-mode.zAngRange, mode.zAngRange);
        float r;

        newPos.y = GetRandom(mode.maxDepth, mode.waterLevel);


        if(mode.Equals(path))
            r = GetRandom(0, ((newPos.y - targetBounds.center.y) * (mode.cameraFov/100)) );
        else
            r = GetRandom(mode.minRadius, mode.maxRadius);

        float theta = GetRandom(mode.minPhi, mode.maxPhi) - 90;
        theta = theta * 3.14f / 180; 

        newPos.x += r * Mathf.Cos(theta);        
        newPos.z += r * Mathf.Sin(theta);

        agent.transform.position = newPos;
        if(mode.Equals(path)) {
            agent.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            agent.transform.LookAt(transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(toAnnotate).center);
            agent.transform.eulerAngles = new Vector3(agent.transform.eulerAngles.x + xRot, agent.transform.eulerAngles.y + yRot, agent.transform.eulerAngles.z + zRot);
        }
    }

    public void GetOthNewPos(GameObject obj)
    {
        Vector3 newPos = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(toAnnotate).center;

        float xRot = GetRandom(-mode.othXAngRange, mode.othXAngRange);
        float yRot = GetRandom(-mode.othYAngRange, mode.othYAngRange);
        float zRot = GetRandom(-mode.othZAngRange, mode.othZAngRange);
        float r = GetRandom(mode.othMinRadius, mode.othMaxRadius);
        float theta = GetRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = GetRandom(mode.othMaxDepth, mode.waterLevel - 0.2f);
        newPos.z += r * Mathf.Sin(theta);
        obj.transform.position = newPos;
        obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
    }

    public List<GameObject> GetRandomObjects(List<GameObject> objects, int amount) {
    	int rand;
    	List<GameObject> objToChose = new List<GameObject>();
    	foreach(GameObject obj in objects) objToChose.Add(obj);
    	while(objToChose.Count > amount) {
    		rand = (int)(GetRandom(0, objToChose.Count));
    		objToChose.RemoveAt(rand);
		}
		return objToChose;
    }

    public void DrawPositions(bool addNoise, bool randomQuarter, bool randomPosition) {
        
        WaterOpacity water = transform.Find("Robot").GetComponent<WaterOpacity>();
        water.waterFog = GetRandom(0.15f, 0.35f);
        water.waterColor = new Color(0.22f, 0.65f, GetRandom(0.5f, 0.8f), 0.5f);

        if(mode.Equals(path)) 
            transform.GetComponent<RandomInitOnMesh>().PutAll(target);
        else 
            transform.GetComponent<RandomInit>().PutAll(randomQuarter, randomPosition, false);

        targetBounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(toAnnotate);

        GetNewPos();
        if (addNoise)
        {
        	List<GameObject> objToChose = GetRandomObjects(otherObjs, numberOfNoiseToGenerate);
            foreach (GameObject obj in objToChose) GetOthNewPos(obj);
            foreach (GameObject obj in otherObjs) obj.SetActive(false);
            foreach (GameObject obj in objToChose) obj.SetActive(true);
        }
        
    }
}
