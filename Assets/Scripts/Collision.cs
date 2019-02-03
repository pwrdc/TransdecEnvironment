using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public float radius = 10f;
    public float dis;

    public GameObject main_object;
    List<Collider> colliders = new List<Collider>();
    Collider min;

    void Update()
    {
        float min_dis = 100;
   
        if (colliders.Count != 0)
        {
            foreach (Collider element in colliders)
            {
                
                float distance = Vector3.Distance(element.transform.position, main_object.transform.position);
                
             
               if (distance< min_dis)
                {
                    min_dis = distance;
                    min = element;
                }
                dis = min_dis;

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

    public float get_distance()
    {
        return dis;
    }
     
}
