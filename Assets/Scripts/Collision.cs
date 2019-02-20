using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    public float dis;

    public GameObject mainObject;
    List<Collider> colliders = new List<Collider>();
    Collider min;

    void Update()
    {
        float minDis = 100;
   
        if (colliders.Count != 0)
        {
            foreach (Collider element in colliders)
            {
                
                float distance = Vector3.Distance(element.transform.position, mainObject.transform.position);
                
             
               if (distance< minDis)
                {
                    minDis = distance;
                    min = element;
                }
                dis = minDis;

            }
            colliders.Clear();
        }

    }

    void OnTriggerEnter(Collider co)
    {
        if (!colliders.Contains(co))
        {
            colliders.Add(co);
        }
    }

    public float getDistance()
    {
        return dis;
    }
     
}
