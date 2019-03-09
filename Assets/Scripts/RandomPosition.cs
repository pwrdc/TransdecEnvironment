using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;

    [System.Serializable]
    public class Gate {
        public List<GameObject> otherObjs = new List<GameObject>();
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
        public List<GameObject> otherObjs = new List<GameObject>();
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

    public Gate gate;
    public Path path;
    public dynamic mode;

    [HideInInspector]
    public Bounds targetBounds;

    
    void OnValidate() {
        /*mode.otherObjs.Clear();
        foreach (Transform child in transform) {
            if (child.gameObject != target && child.gameObject != agent)
                mode.otherObjs.Add(child.gameObject);
        }*/
    }

    float GetRandom(float min, float max)
    {
        System.Random rnd = transform.gameObject.GetComponent<RandomInit>().GetRandomizer();
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
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

        float theta = GetRandom(0, 2 * Mathf.PI);

        newPos.x += r * Mathf.Cos(theta);        
        newPos.z += r * Mathf.Sin(theta);

        agent.transform.position = newPos;
        if(mode.Equals(path)) {
            agent.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            agent.transform.LookAt(transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(target).center);
            agent.transform.eulerAngles = new Vector3(agent.transform.eulerAngles.x + xRot, agent.transform.eulerAngles.y + yRot, agent.transform.eulerAngles.z + zRot);
        }
    }

    public void GetOthNewPos(GameObject obj)
    {
        Vector3 newPos = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(target).center;
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

    public void DrawPositions(bool addNoise, bool randomQuarter, bool randomPosition) {
        
        WaterOpacity water = transform.Find("Robot").GetComponent<WaterOpacity>();
        water.waterFog = GetRandom(0.15f, 0.35f);
        water.waterColor = new Color(0.22f, 0.65f, GetRandom(0.5f, 0.8f), 0.5f);

        if(mode.Equals(path)) 
            transform.GetComponent<RandomInitOnMesh>().PutAll();
        else 
            transform.GetComponent<RandomInit>().PutAll(randomQuarter, randomPosition, false);

        targetBounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(target);
        GetNewPos();
        if (addNoise)
        {
            foreach (GameObject obj in mode.otherObjs) GetOthNewPos(obj);
        }
        
    }
}
