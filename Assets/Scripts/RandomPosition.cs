using System.Collections;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;
    public GameObject[] otherObjs;
    [Range(0.0f, 10.0f)]
    public float minRadius = 3.0f;
    [Range(0.0f, 10.0f)]
    public float maxRadius = 10.0f;
    public float waterLevel = 11.0f;
    [Range(0.0f, 11.0f)]
    public float maxDepth = 4.0f;
    [Range(0.0f, 30.0f)]
    public float xAngRange = 5f;
    [Range(0.0f, 30.0f)]
    public float yAngRange = 5f;
    [Range(0.0f, 30.0f)]
    public float zAngRange = 5f;
    public bool randomQuarter = false;
    //other objects parameters
    public bool othObjects = true;
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

    float getRandom(float min, float max)
    {
        System.Random rnd = GameObject.Find("Academy").GetComponent<RandomInit>().rnd;
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    public void getNewPos()
    {
        Vector3 newPos = target.GetComponent<Renderer>().bounds.center;
        float xRot = getRandom(-xAngRange, xAngRange);
        float yRot = getRandom(-yAngRange, yAngRange);
        float zRot = getRandom(-zAngRange, zAngRange);
        float r = getRandom(minRadius, maxRadius);
        float theta = getRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = getRandom(maxDepth, waterLevel);
        newPos.z += r * Mathf.Sin(theta);
        agent.transform.position = newPos;
        agent.transform.LookAt(target.GetComponent<Renderer>().bounds.center);
        agent.transform.eulerAngles = new Vector3(xRot, agent.transform.eulerAngles.y + yRot, zRot);
    }

    public void getOthNewPos(GameObject obj)
    {
        Vector3 newPos = target.GetComponent<Renderer>().bounds.center;
        float xRot = getRandom(-othXAngRange, othXAngRange);
        float yRot = getRandom(-othYAngRange, othYAngRange);
        float zRot = getRandom(-othZAngRange, othZAngRange);
        float r = getRandom(othMinRadius, othMaxRadius);
        float theta = getRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = getRandom(othMaxDepth, waterLevel);
        newPos.z += r * Mathf.Sin(theta);
        obj.transform.position = newPos;
        obj.transform.LookAt(target.GetComponent<Renderer>().bounds.center);
        obj.transform.eulerAngles = new Vector3(xRot, obj.transform.eulerAngles.y + yRot, zRot);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (randomQuarter)
        {
            GameObject.Find("Academy").GetComponent<RandomInit>().PutAll();
        }
        getNewPos();
        if(othObjects)
        {
            foreach (GameObject obj in otherObjs) getOthNewPos(obj);
        }
    }
}
