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

    float GetRandom(float min, float max)
    {
        System.Random rnd = GameObject.Find("Academy").GetComponent<RandomInit>().rnd;
        float rand = (float)rnd.NextDouble();
        float ret = (max - min) * rand + min;
        return ret;
    }

    public void GetNewPos()
    {
        Vector3 newPos = target.GetComponent<Renderer>().bounds.center;
        float xRot = GetRandom(-xAngRange, xAngRange);
        float yRot = GetRandom(-yAngRange, yAngRange);
        float zRot = GetRandom(-zAngRange, zAngRange);
        float r = GetRandom(minRadius, maxRadius);
        float theta = GetRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = GetRandom(maxDepth, waterLevel);
        newPos.z += r * Mathf.Sin(theta);
        agent.transform.position = newPos;
        agent.transform.LookAt(target.GetComponent<Renderer>().bounds.center);
        agent.transform.eulerAngles = new Vector3(xRot, agent.transform.eulerAngles.y + yRot, zRot);
    }

    public void GetOthNewPos(GameObject obj)
    {
        Vector3 newPos = target.GetComponent<Renderer>().bounds.center;
        float xRot = GetRandom(-othXAngRange, othXAngRange);
        float yRot = GetRandom(-othYAngRange, othYAngRange);
        float zRot = GetRandom(-othZAngRange, othZAngRange);
        float r = GetRandom(othMinRadius, othMaxRadius);
        float theta = GetRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = GetRandom(othMaxDepth, waterLevel);
        newPos.z += r * Mathf.Sin(theta);
        obj.transform.position = newPos;
        obj.transform.LookAt(target.GetComponent<Renderer>().bounds.center);
        obj.transform.eulerAngles = new Vector3(xRot, obj.transform.eulerAngles.y + yRot, zRot);
    }

    public void DrawPosition()
    {
        if (randomQuarter)
        {
            GameObject.Find("Academy").GetComponent<RandomInit>().PutAll();
        }
        GetNewPos();
        if (othObjects)
        {
            foreach (GameObject obj in otherObjs) GetOthNewPos(obj);
        }
    }
}
