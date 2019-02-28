﻿using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RandomPosition : MonoBehaviour
{
    public GameObject agent = null;
    public GameObject target = null;
    public List<GameObject> otherObjs = new List<GameObject>();
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

    void OnValidate() {
        otherObjs.Clear();
        foreach (Transform child in transform) {
            if (child.gameObject != target && child.gameObject != agent)
                otherObjs.Add(child.gameObject);
        }
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
        Bounds bounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(target);
        Vector3 newPos = bounds.center;
        float xRot = GetRandom(-xAngRange, xAngRange);
        float yRot = GetRandom(-yAngRange, yAngRange);
        float zRot = GetRandom(-zAngRange, zAngRange);
        float r = GetRandom(minRadius, maxRadius);
        float theta = GetRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = GetRandom(maxDepth, waterLevel - 0.2f);
        newPos.z += r * Mathf.Sin(theta);
        agent.transform.position = newPos;
        agent.transform.LookAt(bounds.center);
        agent.transform.eulerAngles = new Vector3(agent.transform.eulerAngles.x + xRot, agent.transform.eulerAngles.y + yRot, agent.transform.eulerAngles.z + zRot);
    }

    public void GetOthNewPos(GameObject obj)
    {
        Bounds bounds = transform.Find("Robot").GetComponent<RobotAgent>().GetComplexBounds(target);
        Vector3 newPos = bounds.center;
        float xRot = GetRandom(-othXAngRange, othXAngRange);
        float yRot = GetRandom(-othYAngRange, othYAngRange);
        float zRot = GetRandom(-othZAngRange, othZAngRange);
        float r = GetRandom(othMinRadius, othMaxRadius);
        float theta = GetRandom(0, 2 * Mathf.PI);
        newPos.x += r * Mathf.Cos(theta);
        newPos.y = GetRandom(othMaxDepth, waterLevel - 0.2f);
        newPos.z += r * Mathf.Sin(theta);
        obj.transform.position = newPos;
        obj.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
    }

    public void DrawPositions(bool addNoise) {
        if (randomQuarter)
        {
            GameObject.Find("Academy").GetComponent<RandomInit>().PutAll();
        }
        GetNewPos();
        if (addNoise)
        {
            foreach (GameObject obj in otherObjs) GetOthNewPos(obj);
        }
    }
}
