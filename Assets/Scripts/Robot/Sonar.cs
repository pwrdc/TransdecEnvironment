using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sonar : MonoBehaviour
{
    //Script counts all distances from objects in tasks

    public float viewRadious;
    public Transform tasks;
    private Dictionary<Transform, int> children;

    
    

    private void Start()
    {

        children = new Dictionary<Transform, int>();


        //this read all children in the 1 hierarchy from tasks and give transform of object
        //value 0 in dictonary becouse objects are too far 
        for (int ID = 0; ID < tasks.childCount; ID++)
        {
            children.Add(tasks.GetChild(ID), 0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < children.Count; i++) {

            var element = children.ElementAt(i);

            //if distance beetween object and robot is smaller then viewRadious => Value in dictonary changes to 1
            if (Vector3.Distance(element.Key.position, transform.position) <= viewRadious)
            {
                children[element.Key] = 1;
                
                
            }
            else {
                children[element.Key] = 0;
            }

        }
      
    }

 


   
}
